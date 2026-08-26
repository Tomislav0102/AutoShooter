using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


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
            _myStats = new StatSingle[System.Enum.GetNames(typeof(Stats)).Length];
            for (int i = 0; i < _myStats.Length; i++)
            {
                _myStats[i] = new StatSingle((Stats)i, data.baseStats[(Stats)i]);
            }
        }
    }
    Brain _br;
    public MyDuo<bool, SoSkill> skillPair;
    StatSingle[] _myStats;

    
    public int GetStat(Stats stat) => _myStats[(int)stat].Value;
    public float GetStat(Stats stat, float percentage)
    {
        return data.baseStats[stat] * percentage + GetStat(stat) - data.baseStats[stat];
    }

    public void BuffInjectData(PassDataStats pds)
    {
        switch (pds.change)
        {
            case GenChange.Add:
                int previousValue = _myStats[(int)pds.stat].Value;
                _myStats[(int)pds.stat].buffs.Add(new Buff(pds.value, pds.hasDuration? pds.duration : float.PositiveInfinity));
                print ($"{pds.stat} changed from {previousValue} to {_myStats[(int)pds.stat].Value}");
                break;
            case GenChange.Remove:
                bool buffFound = false;
                foreach (Buff item in _myStats[(int)pds.stat].buffs)
                {
                    if (!(item.bonus == pds.value && float.IsPositiveInfinity(item.duration))) continue;
                    _myStats[(int)pds.stat].buffs.Remove(item);
                    print ($"Buff on {pds.stat} with value {_myStats[(int)pds.stat].Value} is removed.");
                    buffFound = true;
                    break;
                }
                if (!buffFound) print("No buff found, nothing is removed");
                break;
        }
    }
    void Update()
    {
        foreach (StatSingle stat in _myStats)
        {
            int count = stat.buffs.Count;
            for (int i = 0; i < count; i++)
            {
                Buff buff = stat.buffs[i];
                if (float.IsPositiveInfinity(buff.duration)) continue;
                if (buff.duration > 0)
                {
                    buff.duration -= Time.deltaTime;
                    continue;
                }
                stat.buffs.RemoveAt(i);
                if (stat.Value == 0); //only to update 'Value' for debug
            }
        }
    }

    [System.Serializable] //debug only
    class StatSingle
    {
        public Stats stat;
        public List<Buff> buffs;
        int _baseValue;
        public int Value
        {
            get
            {
                int res = _baseValue;
                for (int i = 0; i < buffs.Count; i++)
                {
                    res += buffs[i].bonus;
                }
                valueDisplay = res;
                return res;
            }
        }
        public int valueDisplay;
        

        public StatSingle(Stats stat, int baseValue)
        {
            this.stat = stat;
            _baseValue = baseValue;
            buffs = new List<Buff>();
            if (Value == 0); //only to update 'Value' for debug
        }

    }
    class Buff
    {
        public int bonus;
        public float duration;

        public Buff(int val, float dur = float.PositiveInfinity)
        {
            bonus = val;
            duration = dur;
        }

    }


}


