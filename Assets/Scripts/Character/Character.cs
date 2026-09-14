using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;


public class Character : SerializedMonoBehaviour, IIniBrain
{
    // public enum BuffStack //only to handle stacking (e.g. Inventory buffs don't stack, while Status do)
    // {
    //     Inventory, 
    //     Status, //slowed, wet, cold...
    //     Skill, //e.g. Ultimate increases attack speed for 10 sec
    //     Spell //buffs from cast spells
    // }
    [SerializeField] SoCharacter data;
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            if (data == null) data = Ga.me.defCharacter;
            buffs = new List<Buff>();
            ResetFinalStats();
        }
    }
    Brain _br;
    public Dictionary<Stats, float> statsFinal = new Dictionary<Stats, float>();
    public List<Buff> buffs = new List<Buff>();


    public int GetStat(Stats stat)
    {
        return (int)statsFinal[stat];
    }
    MyDuo<Element, float> GetElDamage()
    {
        MyDuo<Element, float> elDamage = new MyDuo<Element, float>(true);
        elDamage.Add(Element.Physical, GetStat(Stats.ExtraDamPhysical));
        elDamage.Add(Element.Fire, GetStat(Stats.ExtraDamFire));
        elDamage.Add(Element.Ice, GetStat(Stats.ExtraDamIce));
        elDamage.Add(Element.Electricity, GetStat(Stats.ExtraDamElectricity));
        elDamage.Add(Element.Poison, GetStat(Stats.ExtraDamPoison));
        elDamage.Add(Element.Magic, GetStat(Stats.ExtraDamMagic));
        return elDamage;
    }
    public MyDuo<Element, float> GetDamage(Stats statOffensive, MyDuo<Element, float> extraDamage = null)
    {
        MyDuo<Element, float> damage = GetElDamage();
        damage.AddRange(extraDamage);
        float finalValue;
        switch (statOffensive)
        {
            case Stats.MeleeDamage:
            case Stats.RangedDamage:
                finalValue = damage.GetValueByKey(Element.Physical) + GetStat(statOffensive);
                damage.SetValueByKey(Element.Physical, finalValue);
                break;
            case Stats.MagicDamage:
                finalValue = damage.GetValueByKey(Element.Magic) + GetStat(statOffensive);
                damage.SetValueByKey(Element.Magic, finalValue);
                break;
            default:
                print("must use offensive stat");
                return null;
        }

        return damage;
    }


    public void BuffInjectData(PassDataStats pds)
    {
        int count = buffs.Count;
        foreach (PassDataStats.Group group in pds.group)
        {
            Buff buff = group.buff;
            switch (group.change)
            {
                case GenChange.Add:
                    if (group.buffType == BuffType.Percentage && Mathf.Approximately(buff.value, 1f))
                    {
                        print("Buff multiplier is 1X, so its ignored");
                        return;
                    }
                    int previousValue = GetStat(buff.stat);
                    float finalValue = buff.value;
                    switch (group.buffType)
                    {
                        case BuffType.Added:
                            statsFinal[buff.stat] += finalValue;
                            break;
                        case BuffType.Percentage:
                            finalValue = data.baseStats[buff.stat] * (buff.value - 1f);
                            statsFinal[buff.stat] += finalValue;
                            break;
                    }
                    buffs.Add(buff);
                    print($"{buff.stat} changed from {previousValue} to {GetStat(buff.stat)}");
                    break;
                case GenChange.Remove:
                    RemoveBuff(buff);
                    break;
            }

        }
    }

    void Update()
    {
        foreach (Buff item in buffs)
        {
            if (item.permanent || float.IsPositiveInfinity(item.duration)) continue;
            if (item.duration > 0)
            {
                item.duration -= Time.deltaTime;
                continue;
            }
            RemoveBuff(item);
        }
    }
    void RemoveBuff(Buff buffToRemove)
    {
        if (buffToRemove == null || !buffs.Contains(buffToRemove)) return;
        statsFinal[buffToRemove.stat] -= buffToRemove.value;
        buffs.Remove(buffToRemove);
        //to mitigate problem of float precision (baseStats are integers, while finalStats are floats)
        if (buffs.Count == 0) ResetFinalStats(); 
    }
    void ResetFinalStats()
    {
        statsFinal = new Dictionary<Stats, float>();
        foreach (KeyValuePair<Stats, int> item in data.baseStats)
        {
            statsFinal.Add(item.Key, item.Value);
        }
    }
    

}
[System.Serializable]
public class Buff
{
    public Stats stat;
    public float value;
    public bool permanent;
    [HideIf(nameof(permanent))] public float duration;

    public Buff() { }
    public Buff(Stats stat, float val, float dur = float.PositiveInfinity)
    {
        this.stat = stat;
        value = val;
        duration = dur;
        permanent = float.IsPositiveInfinity(duration);
    }
}




// public class Character : MonoBehaviour, IIniBrain
// {
//     // public enum BuffStack //only to handle stacking (e.g. Inventory buffs don't stack, while Status do)
//     // {
//     //     Inventory, 
//     //     Status, //slowed, wet, cold...
//     //     Skill, //e.g. Ultimate increases attack speed for 10 sec
//     //     Spell //buffs from cast spells
//     // }
//     [SerializeField] SoCharacter data;
//     public Brain Br
//     {
//         get => _br;
//         set
//         {
//             _br = value;
//             if (data == null) data = Ga.me.defCharacter;
//             _pair = new MyDuo<Stats, StatSingle>();
//             _length= System.Enum.GetNames(typeof(Stats)).Length;
//             for (int i = 0; i < _length; i++)
//             {
//                 Stats st = (Stats)i;
//                 _pair.Add(st, new StatSingle(data.baseStats[st]));
//             }
//         }
//     }
//     Brain _br;
//     public MyDuo<Stats, StatSingle> _pair;
//     int _length;
//
//     public int GetStat(Stats stat) => _pair.GetValueByKey(stat).Value();
//     MyDuo<Element, float> GetElDamage()
//     {
//         MyDuo<Element, float> elDamage = new MyDuo<Element, float>(true);
//         elDamage.Add(Element.Physical, GetStat(Stats.ExtraDamPhysical));
//         elDamage.Add(Element.Fire, GetStat(Stats.ExtraDamFire));
//         elDamage.Add(Element.Ice, GetStat(Stats.ExtraDamIce));
//         elDamage.Add(Element.Electricity, GetStat(Stats.ExtraDamElectricity));
//         elDamage.Add(Element.Poison, GetStat(Stats.ExtraDamPoison));
//         elDamage.Add(Element.Magic, GetStat(Stats.ExtraDamMagic));
//         return elDamage;
//     }
//     public MyDuo<Element, float> GetDamage(Stats statOffensive)
//     {
//         MyDuo<Element, float> damage = GetElDamage();
//         float finalValue;
//         switch (statOffensive)
//         {
//             case Stats.MeleeDamage:
//             case Stats.RangedDamage:
//                 finalValue = damage.GetValueByKey(Element.Physical) + GetStat(statOffensive);
//                 damage.SetValueByKey(Element.Physical, finalValue);
//                 break;
//             case Stats.MagicDamage:
//                 finalValue = damage.GetValueByKey(Element.Magic) + GetStat(statOffensive);
//                 damage.SetValueByKey(Element.Magic, finalValue);
//                 break;
//             default:
//                 print("must use offensive stat");
//                 return null;
//         }
//         
//         return  damage;
//     }
//     
//
//     public void BuffInjectData(PassDataStats pds)
//     {
//         foreach (PassDataStats.Group group in pds.group)
//         {
//             StatSingle ss = _pair.GetValueByKey(group.stat);
//             switch (group.change)
//             {
//                 case GenChange.Add:
//                     if (group.buffType == BuffType.Percentage && Mathf.Approximately(group.value, 1f))
//                     {
//                         print("Buff multiplier is 1X, so its ignored");
//                         return;
//                     }
//                     int previousValue = ss.Value();
//                     ss.buffs.Add(new Buff(group.buffType, group.value, group.hasDuration? group.duration : float.PositiveInfinity));
//                     print ($"{group.stat} changed from {previousValue} to {ss.Value()}");
//                     break;
//                 case GenChange.Remove:
//                     bool buffFound = false;
//                     foreach (Buff item in ss.buffs)
//                     {
//                         if (!(Mathf.Approximately(item.value, group.value) && float.IsPositiveInfinity(item.duration))) continue;
//                         ss.buffs.Remove(item);
//                         print ($"Buff on {group.stat} with value {ss.Value()} is removed.");
//                         buffFound = true;
//                         break;
//                     }
//                     if (!buffFound) print("No buff found, nothing is removed");
//                     break;
//             }
//             
//         }
//     }
//     void Update() 
//     {
//         buffDecay();
//         return;
//         
//         void buffDecay()
//         {
//             for (int i = 0; i < _length; i++)
//             {
//                 StatSingle ss = _pair.GetValue(i);
//                 int count = ss.buffs.Count;
//                 for (int j = 0; j < count; j++)
//                 {
//                     Buff buff = ss.buffs[j];
//                     if (float.IsPositiveInfinity(buff.duration)) continue;
//                     if (buff.duration > 0)
//                     {
//                         buff.duration -= Time.deltaTime;
//                         continue;
//                     }
//                     ss.buffs.RemoveAt(j);
//                 }
//             }
//         }
//     }
//
//     [System.Serializable] //debug only
//     public class StatSingle
//     {
//         public List<Buff> buffs;
//         public int baseValue;
//         public int Value()
//         {
//             int res = baseValue;
//             for (int i = 0; i < buffs.Count; i++)
//             {
//                 switch (buffs[i].buffType)
//                 {
//                     case BuffType.Added:
//                         res += (int)buffs[i].value;
//                         break;
//                     case BuffType.Percentage:
//                         res += (int)(buffs[i].value * baseValue);
//                         break;
//                 }
//             }
//             valueDisplay = res;
//             return res;
//         }
//         public int valueDisplay;
//         
//         public StatSingle(int baseValue)
//         {
//             this.baseValue = baseValue;
//             buffs = new List<Buff>();
//         }
//
//     }
//     public class Buff
//     {
//         public float value;
//         public float duration;
//         public BuffType buffType;
//
//         public Buff(BuffType bType, float val, float dur = float.PositiveInfinity)
//         {
//             buffType = bType;
//             value = val;
//             duration = dur;
//         }
//
//     }
//
//
// }
