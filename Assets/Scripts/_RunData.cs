using System.Collections.Generic;
using UnityEngine;

public class RunData
{
    public Dictionary<string, int> dic;
    public MyDuo<Status.Effect, int> enUnderEffect;
    //int[] is value for Stats[] but only Resists
    public MyDuo<Status.Effect, int[]> enEffectVulnerabilities;
    int _elementsLength;
    public RunData()
    {
        enUnderEffect = new MyDuo<Status.Effect, int>();
        Status.OnEffectChange += CallEvStatusEffects;
        int length = System.Enum.GetNames(typeof(Status.Effect)).Length;
        for (int i = 0; i < length; i++)
        {
            enUnderEffect.Add((Status.Effect)i, 0);
        }
        _elementsLength =  System.Enum.GetNames(typeof(Element)).Length;
        enEffectVulnerabilities = new MyDuo<Status.Effect, int[]>();
        enEffectVulnerabilities.Add(Status.Effect.Frozen, new int[_elementsLength]);
        enEffectVulnerabilities.Add(Status.Effect.Stunned, new int[_elementsLength]);
        enEffectVulnerabilities.Add(Status.Effect.Burning_dot, new int[_elementsLength]);
    }

    void CallEvStatusEffects(Brain brain, Status.Effect effect, bool on)
    {
        switch (effect)
        {
            case Status.Effect.Burning_dot:
                int prevValue = enUnderEffect.GetValueByKey(Status.Effect.Burning_dot);
                if (on) prevValue++;
                else prevValue--;
                enUnderEffect.SetValueByKey(Status.Effect.Burning_dot, prevValue);
                break;
        }
    }

    public void AddAllVulnerabilities(Status.Effect effect, int valueToAdd)
    {
        int[] previousValues = Ga.me.runData.enEffectVulnerabilities.GetValueByKey(effect);
        for (int i = 0; i < _elementsLength; i++)
        {
            previousValues[i] += valueToAdd;
            Ga.me.runData.enEffectVulnerabilities.SetValueByKey(effect, previousValues);
        }
    }
    public void PrintVulnerabilities()
    {
        for (int i = 0; i < enEffectVulnerabilities.Length(); i++)
        {
            for (int j = 0; j < enEffectVulnerabilities.GetValue(i).Length; j++)
            {
                Debug.Log($"effect: {enEffectVulnerabilities.GetKey(i)} at element {(Element)j}({enEffectVulnerabilities.GetValue(i)[j]})");
            }
        }
    }

}

/// <summary>
/// For simpler use cases. Real buffs need other classes
/// </summary>
[System.Serializable]
public struct ValueCalc
{
    public float Result
    {
        get
        {
            if (_buffsSet.Count > 0)
            {
                int set = 0;
                for (int i = 0; i < _buffsSet.Count; i++)
                {
                    set += _buffsSet[i];
                }
                return set;
            }

            float perc = 1f;
            foreach (int f in _buffsPercentage)
            {
                perc += f * 0.01f;
            }
          //  Debug.Log($"before {perc}");
            if (perc < 0f) perc = 0f;
          //  Debug.Log($"after {perc}");
            
            int add = 0;
            for (int i = 0; i < _buffsAdded.Count; i++)
            {
                add += _buffsAdded[i];
            }
            return _baseValue * perc + add;
        }
    }
    int _baseValue;
    List<int> _buffsAdded;
    List<int> _buffsPercentage;
    List<int> _buffsSet;

    public ValueCalc(int baseValue)
    {
        _baseValue = baseValue;
        _buffsAdded = new List<int>();
        _buffsPercentage = new List<int>();
        _buffsSet = new List<int>();
    }
    public void ChangeBuff(GenChange change, BuffType buffType, int value)
    {
        switch (change)
        {
            case GenChange.Add:
                switch (buffType)
                {
                    case BuffType.Added:
                        _buffsAdded.Add(value);
                        break;
                    case BuffType.Percentage:
                        _buffsPercentage.Add(value);
                        break;
                    case BuffType.Set:
                        _buffsSet.Add(value);
                        break;
                }
                break;
            case GenChange.Remove:
                switch (buffType)
                {
                    case BuffType.Added:
                        if (_buffsAdded.Contains(value)) _buffsAdded.Remove(value);
                        break;
                    case BuffType.Percentage:
                        if (_buffsPercentage.Contains(value)) _buffsPercentage.Remove(value);
                        break;
                    case BuffType.Set:
                        if (_buffsSet.Contains(value)) _buffsSet.Remove(value);
                        break;
                }
                break;
        }
    }
}