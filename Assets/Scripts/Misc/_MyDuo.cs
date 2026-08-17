using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class MyDuo<K, V>
{
    [BoxGroup("Main")][field: SerializeField] K[] Key { get; set; } 
    [BoxGroup("Main")][field: SerializeField] V[] Value { get; set; }
    public int Length()
    {
        return Key == null ? 0 : Key.Length;
    }

    string _prefixDebug() => $"MyDou<{Key},{Value}> debug info:\n";
    K[] _bufferKey;
    V[] _bufferValue;
    bool _canHaveDuplicateKeys;
    bool _canHaveDuplicateValues;

    #region CONSTRUCTORS
    
    public MyDuo(bool canHaveDuplicateKeys = false, bool canHaveDuplicateValues = true)
    {
        Key = Array.Empty<K>();
        Value = Array.Empty<V>();
        _canHaveDuplicateKeys = canHaveDuplicateKeys;
        _canHaveDuplicateValues = canHaveDuplicateValues;
    }
    public MyDuo(Dictionary<K, V> fromDictionary, bool canHaveDuplicateKeys = false, bool canHaveDuplicateValues = true)
    {
        int count = 0;
        Key = new K[fromDictionary.Count];
        Value = new V[fromDictionary.Count];
        foreach (KeyValuePair<K,V> item in fromDictionary)
        {
            Key[count] = item.Key;
            Value[count] = item.Value;
            count++;
        }
        _canHaveDuplicateKeys = canHaveDuplicateKeys;
        _canHaveDuplicateValues = canHaveDuplicateValues;
    }

    public MyDuo(K[] keys, V[] values)
    {
        if (keys.Length != values.Length) return;
        Key = keys;
        Value = values;

        for (int i = 0; i < Length(); i++)
        {
            if (isDuplicateKey(Key[i])) _canHaveDuplicateKeys = true;
            if (isDuplicateValue(Value[i])) _canHaveDuplicateValues = true;
        }

        bool isDuplicateKey(K key)
        {
            bool foundMyself = false;
            for (int i = 0; i < Length(); i++)
            {
                if (Key[i] == null && key == null) //if value is null, 'Equals()' throws an error
                {
                    if (foundMyself) return true;
                    foundMyself = true;
                    continue;
                }
                if (Key[i].Equals(key))
                {
                    if (foundMyself) return true;
                    foundMyself = true;
                }
            }
            return false;
        }
        bool isDuplicateValue(V val)
        {
            bool foundMyself = false;
            for (int i = 0; i < Length(); i++)
            {
                if (Value[i] == null && val == null)
                {
                    if (foundMyself) return true;
                    foundMyself = true;
                    continue;
                }
                if (Value[i].Equals(val))
                {
                    if (foundMyself) return true;
                    foundMyself = true;
                }
            }
            return false;
        }
    }
    #endregion

    #region MAINPULATE DATA
    public void Add(K key, V value)
    {
        if (!_canHaveDuplicateKeys)
        {
            for (int i = 0; i < Key.Length; i++)
            {
                if (Key[i].Equals(key))
                {
                    Debug.LogError($"{_prefixDebug()}key '{key}' already present");
                    return;
                }
            }
        }
        if (!_canHaveDuplicateValues)
        {
            for (int i = 0; i < Value.Length; i++)
            {
                if (Value[i].Equals(value))
                {
                    Debug.LogError($"{_prefixDebug()}value '{value}' already present");
                    return;
                }
            }
        }
        
        _bufferKey = new K[Key.Length + 1];
        _bufferKey[^1] = key;
        for (int i = 0; i < Key.Length; i++)
        {
            _bufferKey[i] = Key[i];
        }
        
        _bufferValue = new V[Value.Length + 1];
        _bufferValue[^1] = value;
        for (int i = 0; i < Value.Length; i++)
        {
            _bufferValue[i] = Value[i];
        }
        
        Key = _bufferKey;
        Value = _bufferValue;
    }
    public void Remove(int index)
    {
        if (Key == null || Key.Length < index)
        {
            Debug.LogError($"{_prefixDebug()}Can't remove");
            return;
        }

        int counter = 0;
        _bufferKey = new K[Key.Length - 1];
        _bufferValue = new V[Value.Length - 1];
        for (int i = 0; i < Key.Length; i++)
        {
            if (i == index)
            {
                continue;
            }
            _bufferKey[counter] = Key[i];
            _bufferValue[counter] = Value[i];
            counter++;
        }
        
        Key = _bufferKey;
        Value = _bufferValue;
    }
    public void Remove(K key)
    {
        if (Key == null || Key.Length == 0 || !ContainsKey(key))
        {
            Debug.LogError($"{_prefixDebug()}Can't remove");
            return;
        }
        
        int counter = 0;
        _bufferKey = new K[Key.Length - 1];
        _bufferValue = new V[Value.Length - 1];
        for (int i = 0; i < Key.Length; i++)
        {
            if (Key[i].Equals(key))
            {
                continue;
            }
            _bufferKey[counter] = Key[i];
            _bufferValue[counter] = Value[i];
            counter++;
        }
        
        Key = _bufferKey;
        Value = _bufferValue;
    }
    public void Remove(V value, bool removeFirst = true)
    {
        if (Value == null || Value.Length == 0 || !ContainsValue(value)) 
        {
            Debug.LogError($"{_prefixDebug()}Can't remove");
            return;
        }
        
        int counter = 0;
        _bufferKey = new K[Key.Length - 1];
        _bufferValue = new V[Value.Length - 1];
        for (int i = 0; i < Value.Length; i++)
        {
            if (Value[i].Equals(value))
            {
                continue;
            }
            _bufferKey[counter] = Key[i];
            _bufferValue[counter] = Value[i];
            counter++;
        }
        
        Key = _bufferKey;
        Value = _bufferValue;
    }
    #endregion

    #region GET DATA
    public K GetKey(int index) => Key[index];
    public V GetValue(int index) => Value[index];
    public V GetValueByKey(K key)
    {
        for (int i = 0; i < Key.Length; i++)
        {
            if (Key[i].Equals(key))
            {
                return Value[i];
            }
        }
        Debug.LogError($"{_prefixDebug()}no key '{key}' present");
        return default;
    }
    public K GetKeyByValue(V val)
    {
        for (int i = 0; i < Value.Length; i++)
        {
            if (Value[i].Equals(val))
            {
                return Key[i];
            }
        }
        Debug.LogError($"{_prefixDebug()}no value '{val}' present");
        return default;
    }
    public bool TryGetValueByKey(K key, out V value)
    {
        for (int i = 0; i < Key.Length; i++)
        {
            if (Key[i].Equals(key))
            {
                value = Value[i];
                return true;
            }
        }
        Debug.LogError($"{_prefixDebug()}no key '{key}' present");
        value = default;
        return false;
    }
    public bool TryGetKeyByValue(V val, out K key)
    {
        for (int i = 0; i < Value.Length; i++)
        {
            if (Value[i].Equals(val))
            {
                key = Key[i];
                return true;
            }
        }
        Debug.LogError($"{_prefixDebug()}no value '{val}' present");
        key = default;
        return false;
    } 
    #endregion

    #region HELPERS
    public Dictionary<K, V> AsDictionary()
    {
        Dictionary<K, V> dictionary = new Dictionary<K, V>();
        for (int i = 0; i < Key.Length; i++)
        {
            dictionary.Add(Key[i], Value[i]);
        }
        return dictionary;
    }
    bool ContainsKey(K key)
    {
        foreach (K item in Key)
        {
            if (item.Equals(key)) return true;
        }
        return false;
    }
    bool ContainsValue(V val)
    {
        foreach (V item in Value)
        {
            if (item.Equals(val)) return true;
        }
        return false;
    }
    #endregion

    #region DEBUGS
    public void DebugPrint()
    {
        string st = "";
        if (Key == null)
        {
            st = "Class isn't initialized";
        }
        else
        {
            st += $"{_prefixDebug()} length is {Length()}\n";
            for (int i = 0; i < Length(); i++)
            {
                st += $"{i} -- Key is {Key[i]}: Value is {Value[i]}\n";
            }
        }
        Debug.Log(st);
    }
    #endregion

}
