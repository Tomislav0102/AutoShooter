using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class Character : MonoBehaviour, IInit
{
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
            IsReady = true;
        }
    }
    Brain _br;
    [ReadOnly] public bool IsReady { get; set; }

    StatSingle[] _myStats;

    public void ChangeStat(Stats statToChange, GenChange change, int amount)
    {
        List<int> modifiers = _myStats[(int)statToChange].modifiers;
        switch (change)
        {
            case GenChange.Add:
                modifiers.Add(amount);
                break;
            case GenChange.Remove:
                if (modifiers.Contains(amount)) modifiers.Remove(amount);
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


