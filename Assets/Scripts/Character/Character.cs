using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class Character : MonoBehaviour, IInit
{
    public enum BuffType
    {
        Inventory, 
        Status, //slowed, wet, cold...
        Skill, //e.g. Ultimate increases attack speed for 10 sec
        Spell //buffs from cast spells
    }
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
                _myStats[i] = new StatSingle(data.stats[(Stats)i]);
            }
            IsInitialized = true;
        }
    }
    Brain _br;
    StatSingle[] _myStats;
    [ReadOnly] public bool IsInitialized { get; set; }
    public int GetStat(Stats stat) => _myStats[(int)stat].Value;

    public void ChangeStat(BuffType bType, Stats stat, int value)
    {
        print ($"{stat} is {_myStats[(int)stat].Value}");
        _myStats[(int)stat].buffs.Add(new Buff(bType, value));
        print ($"{stat} increased to {_myStats[(int)stat].Value}");
    }


    
    class StatSingle
    {
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
                return res;
            }
        }
        

        public StatSingle(int baseValue)
        {
            _baseValue = baseValue;
            buffs = new List<Buff>();
        }

    }
    public class Buff
    {
        public BuffType buffType;
        public int bonus;
        public float duration;

        public Buff(BuffType bType, int val = 1, float dur = -1)
        {
            buffType = bType;
            bonus = val;
            duration = dur;
        }

    }


}


