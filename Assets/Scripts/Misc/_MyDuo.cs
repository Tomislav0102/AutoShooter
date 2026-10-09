using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class MyDuo<K, V> 
{
    [field: SerializeField] K[] Key { get; set; } 
    [field: SerializeField] V[] Value { get; set; }
    public int Length()
    {
        return Key == null ? 0 : Key.Length;
    }

    string _prefixDebug() => $"MyDou<{Key},{Value}> debug info:\n";
    bool _debug = false;
    K[] _bufferKey;
    V[] _bufferValue;
    public bool CanHaveDuplicateKeys { get; private set; }
    public bool CanHaveDuplicateValues { get; private set; }

    
    
    #region CONSTRUCTORS
    
    public MyDuo(bool canHaveDuplicateKeys = true, bool canHaveDuplicateValues = true)
    {
        Key = Array.Empty<K>();
        Value = Array.Empty<V>();
        CanHaveDuplicateKeys = canHaveDuplicateKeys;
        CanHaveDuplicateValues = canHaveDuplicateValues;
    }
    public MyDuo(Dictionary<K, V> fromDictionary, bool canHaveDuplicateKeys = true, bool canHaveDuplicateValues = true)
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
        CanHaveDuplicateKeys = canHaveDuplicateKeys;
        CanHaveDuplicateValues = canHaveDuplicateValues;
    }

    public MyDuo(K[] keys, V[] values)
    {
        if (keys.Length != values.Length) return;
        Key = keys;
        Value = values;

        for (int i = 0; i < Length(); i++)
        {
            if (isDuplicateKey(Key[i])) CanHaveDuplicateKeys = true;
            if (isDuplicateValue(Value[i])) CanHaveDuplicateValues = true;
        }
        return;

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

    #region SET DATA
    public void Add(K key, V value)
    {
        if (!CanHaveDuplicateKeys)
        {
            for (int i = 0; i < Key.Length; i++)
            {
                if (Key[i].Equals(key))
                {
                    if (_debug) Debug.LogError($"{_prefixDebug()}key '{key}' already present");
                    return;
                }
            }
        }
        if (!CanHaveDuplicateValues)
        {
            for (int i = 0; i < Value.Length; i++)
            {
                if (Value[i].Equals(value))
                {
                    if (_debug) Debug.LogError($"{_prefixDebug()}value '{value}' already present");
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
    public void AddRange(MyDuo<K, V> anotherDuo)
    {
        if (anotherDuo == null || anotherDuo.Length() == 0)
        {
            if (_debug) Debug.LogWarning("Duo to add is null or empty");
            return;
        }
        if (!CanHaveDuplicateKeys)
        {
            for (int i = 0; i < Length(); i++)
            {
                for (int j = 0; j < anotherDuo.Length(); j++)
                {
                    if (!Key[i].Equals(anotherDuo.Key[j])) continue;
                    if (_debug) Debug.LogError($"{anotherDuo.Key[j]} already present");
                    return;
                }
            }
        }
        if (!CanHaveDuplicateValues)
        {
            for (int i = 0; i < Length(); i++)
            {
                for (int j = 0; j < anotherDuo.Length(); j++)
                {
                    if (!Value[i].Equals(anotherDuo.Value[j])) continue;
                    if (_debug) Debug.LogError($"{anotherDuo.Value[j]} already present");
                    return;
                }
            }
        }
        
        _bufferKey = new K[Length() + anotherDuo.Length()];
        _bufferValue = new V[Length() + anotherDuo.Length()];
        for (int i = 0; i < Length(); i++)
        {
            _bufferKey[i] =  Key[i];
            _bufferValue[i] =  Value[i];
        }
        for (int i = Length(); i < _bufferKey.Length; i++)
        {
            _bufferKey[i] = anotherDuo.GetKey(i - Length());
            _bufferValue[i] =  anotherDuo.GetValue(i - Length());
        }
        Key = _bufferKey;
        Value = _bufferValue;
    }
    public void Remove(int index)
    {
        if (Key == null || Key.Length < index)
        {
            if (_debug) Debug.LogError($"{_prefixDebug()}Can't remove");
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
    public void RemoveByKey(K key)
    {
        if (Key == null || Key.Length == 0 || !ContainsKey(key))
        {
            if (_debug) Debug.LogError($"{_prefixDebug()}Can't remove");
            return;
        }
        
        int counter = 0;
        _bufferKey = new K[Key.Length - 1];
        _bufferValue = new V[Value.Length - 1];
        bool hasRemoved = false;
        for (int i = 0; i < Key.Length; i++)
        {
            if (!hasRemoved && Key[i].Equals(key))
            {
                hasRemoved = true;
                continue;
            }
            _bufferKey[counter] = Key[i];
            _bufferValue[counter] = Value[i];
            counter++;
        }
        
        Key = _bufferKey;
        Value = _bufferValue;
    }
    public void RemoveByValue(V value)
    {
        if (Value == null || Value.Length == 0 || !ContainsValue(value)) 
        {
            if (_debug) Debug.LogError($"{_prefixDebug()}Can't remove");
            return;
        }
        
        int counter = 0;
        _bufferKey = new K[Key.Length - 1];
        _bufferValue = new V[Value.Length - 1];
        bool hasRemoved = false;
        for (int i = 0; i < Value.Length; i++)
        {
            if (!hasRemoved && Value[i].Equals(value))
            {
                hasRemoved = true;
                continue;
            }
            _bufferKey[counter] = Key[i];
            _bufferValue[counter] = Value[i];
            counter++;
        }
        
        Key = _bufferKey;
        Value = _bufferValue;
    }
    public void SetValue(int index, V value) => Value[index] = value;
    public void SetValueByKey(K key, V value)
    {
        for (int i = 0; i < Length(); i++)
        {
            if (!Key[i].Equals(key)) continue;
            Value[i] = value;
            return;
        }
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
        if (_debug) Debug.LogError($"{_prefixDebug()}no key '{key}' present");
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
        if (_debug) Debug.LogError($"{_prefixDebug()}no value '{val}' present");
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
        if (_debug) Debug.LogError($"{_prefixDebug()}no key '{key}' present");
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
        if (_debug) Debug.LogError($"{_prefixDebug()}no value '{val}' present");
        key = default;
        return false;
    }
    public bool HasKey(K key)
    {
        for (int i = 0; i < Length(); i++)
        {
            if (Key[i].Equals(key)) return  true;
        }
        return false;
    }
    public bool HasValue(V value)
    {
        for (int i = 0; i < Length(); i++)
        {
            if (Value[i].Equals(value)) return  true;
        }
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
