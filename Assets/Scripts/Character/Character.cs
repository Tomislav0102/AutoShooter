using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;


public class Character : MonoBehaviour, IIniBrain
{
    // public enum BuffType //only to handle stacking (e.g. Inventory buffs don't stack, while Status do)
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
            _pair = new MyDuo<Stats, StatSingle>();
            _length= System.Enum.GetNames(typeof(Stats)).Length;
            for (int i = 0; i < _length; i++)
            {
                Stats st = (Stats)i;
                _pair.Add(st, new StatSingle(data.baseStats[st]));
            }
        }
    }
    Brain _br;
    public MyDuo<Stats, StatSingle> _pair;
    int _length;

    public int GetStat(Stats stat) => _pair.GetValueByKey(stat).Value();
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
    public MyDuo<Element, float> GetDamage(Stats statOffensive)
    {
        MyDuo<Element, float> damage = GetElDamage();
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
        
        return  damage;
    }
    

    public void BuffInjectData(PassDataStats pds)
    {
        foreach (PassDataStats.Group group in pds.group)
        {
            StatSingle ss = _pair.GetValueByKey(group.stat);
            switch (group.change)
            {
                case GenChange.Add:
                    if (group.buffType == BuffType.Percentage && Mathf.Approximately(group.value, 1f))
                    {
                        print("Buff multiplier is 1X, so its ignored");
                        return;
                    }
                    int previousValue = ss.Value();
                    ss.buffs.Add(new Buff(group.buffType, group.value, group.hasDuration? group.duration : float.PositiveInfinity));
                    print ($"{group.stat} changed from {previousValue} to {ss.Value()}");
                    break;
                case GenChange.Remove:
                    bool buffFound = false;
                    foreach (Buff item in ss.buffs)
                    {
                        if (!(Mathf.Approximately(item.value, group.value) && float.IsPositiveInfinity(item.duration))) continue;
                        ss.buffs.Remove(item);
                        print ($"Buff on {group.stat} with value {ss.Value()} is removed.");
                        buffFound = true;
                        break;
                    }
                    if (!buffFound) print("No buff found, nothing is removed");
                    break;
            }
            
        }
    }
    void Update() 
    {
        buffDecay();
        return;
        
        void buffDecay()
        {
            for (int i = 0; i < _length; i++)
            {
                StatSingle ss = _pair.GetValue(i);
                int count = ss.buffs.Count;
                for (int j = 0; j < count; j++)
                {
                    Buff buff = ss.buffs[j];
                    if (float.IsPositiveInfinity(buff.duration)) continue;
                    if (buff.duration > 0)
                    {
                        buff.duration -= Time.deltaTime;
                        continue;
                    }
                    ss.buffs.RemoveAt(j);
                }
            }
        }
    }

    [System.Serializable] //debug only
    public class StatSingle
    {
        public List<Buff> buffs;
        public int baseValue;
        public int Value()
        {
            int res = baseValue;
            for (int i = 0; i < buffs.Count; i++)
            {
                switch (buffs[i].buffType)
                {
                    case BuffType.Added:
                        res += (int)buffs[i].value;
                        break;
                    case BuffType.Percentage:
                        res += (int)(buffs[i].value * baseValue);
                        break;
                }
            }
            valueDisplay = res;
            return res;
        }
        public int valueDisplay;
        
        public StatSingle(int baseValue)
        {
            this.baseValue = baseValue;
            buffs = new List<Buff>();
        }

    }
    public class Buff
    {
        public float value;
        public float duration;
        public BuffType buffType;

        public Buff(BuffType bType, float val, float dur = float.PositiveInfinity)
        {
            buffType = bType;
            value = val;
            duration = dur;
        }

    }


}


// public class Character : MonoBehaviour, IIniBrain
// {
//     // public enum BuffType //only to handle stacking (e.g. Inventory buffs don't stack, while Status do)
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
//             int length = System.Enum.GetNames(typeof(Stats)).Length;
//             _pair = new MyDuo<Stats, StatSingle>();
//             StatSingle[] _myStats = new StatSingle[length];
//             for (int i = 0; i < length; i++)
//             {
//                 _myStats[i] = new StatSingle(data.baseStats[(Stats)i]);
//                 _pair.Add((Stats)i, _myStats[i]);
//             }
//         }
//     }
//     Brain _br;
//     StatSingle[] _myStats;
//     MyDuo<Stats, StatSingle> _pair;
//
//     
//     public int GetStat(Stats stat) => _myStats[(int)stat].Value();
//     public float GetStat(Stats stat, float percentage)
//     {
//         return data.baseStats[stat] * percentage + GetStat(stat) - data.baseStats[stat];
//     }
//     public float GetElDamage(float percentage = 1f)
//     {
//         float fl = 0f;
//         fl += _myStats[(int)Stats.ExtraDamPhysical].Value();
//         return fl;
//     }
//
//     public void BuffInjectData(PassDataStats pds)
//     {
//         foreach (PassDataStats.Group group in pds.group)
//         {
//             switch (group.change)
//             {
//                 case GenChange.Add:
//                     int previousValue = _myStats[(int)group.stat].Value();
//                     _myStats[(int)group.stat].buffs.Add(new Buff(group.value, group.hasDuration? group.duration : float.PositiveInfinity));
//                     print ($"{group.stat} changed from {previousValue} to {_myStats[(int)group.stat].Value()}");
//                     break;
//                 case GenChange.Remove:
//                     bool buffFound = false;
//                     foreach (Buff item in _myStats[(int)group.stat].buffs)
//                     {
//                         if (!(item.bonus == group.value && float.IsPositiveInfinity(item.duration))) continue;
//                         _myStats[(int)group.stat].buffs.Remove(item);
//                         print ($"Buff on {group.stat} with value {_myStats[(int)group.stat].Value()} is removed.");
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
//         foreach (StatSingle stat in _myStats)
//         {
//             int count = stat.buffs.Count;
//             for (int i = 0; i < count; i++)
//             {
//                 Buff buff = stat.buffs[i];
//                 if (float.IsPositiveInfinity(buff.duration)) continue;
//                 if (buff.duration > 0)
//                 {
//                     buff.duration -= Time.deltaTime;
//                     continue;
//                 }
//                 stat.buffs.RemoveAt(i);
//                 if (stat.Value() == 0); //only to update 'Value' for debug
//             }
//         }
//     }
//
//     [System.Serializable] //debug only
//     class StatSingle
//     {
//         public Stats stat;
//         public List<Buff> buffs;
//         public int baseValue;
//         public int BuffValue()
//         {
//             int res = 0;
//             for (int i = 0; i < buffs.Count; i++)
//             {
//                 res += buffs[i].bonus;
//             }
//             return res;
//         }
//         public int Value()
//         {
//             int res = baseValue + BuffValue();
//             valueDisplay = res;
//             return res;
//         }
//         public int valueDisplay;
//         
//
//         public StatSingle(Stats stat, int baseValue)
//         {
//             this.stat = stat;
//             this.baseValue = baseValue;
//             buffs = new List<Buff>();
//             if (Value() == 0); //only to update 'Value' for debug
//         }
//         public StatSingle(int baseValue)
//         {
//             this.baseValue = baseValue;
//             buffs = new List<Buff>();
//             if (Value() == 0); //only to update 'Value' for debug
//         }
//
//     }
//     class Buff
//     {
//         public int bonus;
//         public float duration;
//
//         public Buff(int val, float dur = float.PositiveInfinity)
//         {
//             bonus = val;
//             duration = dur;
//         }
//
//     }
//
//
// }
