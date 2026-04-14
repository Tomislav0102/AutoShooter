using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class Character : IInit
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _myStats = new StatSingle[System.Enum.GetNames(typeof(Stats)).Length];
            for (int i = 0; i < _myStats.Length; i++)
            {
                _myStats[i] = new StatSingle(100);
            }
            IsReady = true;
        }
    }
    Brain _br;
    public bool IsReady { get; set; }

    StatSingle[] _myStats;

    public Character(Brain brain)
    {
        Br = brain;
    }
    
    public void ChangeStat(Stats statToChange, GenChange change, int amount)
    {
        List<int> modifiers = _myStats[(int)statToChange].modifiers;
        switch (change)
        {
            case GenChange.Add:
                modifiers.Add(amount);
                break;
            case GenChange.Remove:
                int index = -1;
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i] == amount)
                    {
                        index = i;
                    }
                }
                if (index > 0) modifiers.RemoveAt(index);
                break;
        }
    }

    public int GetStat(Stats stat) => _myStats[(int)stat].Value;
    
    class StatSingle
    {
        int _baseValue;
        public List<int> modifiers;
        public int Value
        {
            get
            {
                int res = _baseValue;
                foreach (int item in modifiers)
                {
                    res+= item;
                }

                return res;
            }
        }
        

        public StatSingle(int baseValue)
        {
            _baseValue = baseValue;
            modifiers = new List<int>();
        }
    }

}


