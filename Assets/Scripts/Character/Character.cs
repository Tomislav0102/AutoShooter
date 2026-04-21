using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;


public class Character : MonoBehaviour, IInit
{
    public enum ModType
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
    [ReadOnly] public bool IsInitialized { get; set; }

    StatSingle[] _myStats;
    public int GetStat(Stats stat) => _myStats[(int)stat].Value;

    public void ChangeStat(Stats statToChange, GenChange change, ModType modType, int amount)
    {
        // switch (change)
        // {
        //     case GenChange.Add:
        //         switch (modType)
        //         {
        //             case ModType.Inventory:
        //                 if (_myStats[(int)statToChange].bonuses[ModType.Inventory] < amount)
        //                 {
        //                     _myStats[(int)statToChange].bonuses[ModType.Inventory] = amount;
        //                 } 
        //                 break;
        //             default:
        //                 _myStats[(int)statToChange].bonuses[modType] += amount;
        //                 break;
        //         }
        //         break;
        //     
        //     case GenChange.Remove:
        //         if (_myStats[(int)statToChange].bonuses[modType] >= amount) _myStats[(int)statToChange].bonuses[modType] -= amount;
        //         break;
        // }
    }

    
    class StatSingle
    {
        public List<MyMod> mods;
        int _baseValue;
        public int Value
        {
            get
            {
                int res = _baseValue;
                for (int i = 0; i < mods.Count; i++)
                {
                    res += mods[i].bonus;
                }
                return res;
            }
        }
        

        public StatSingle(int baseValue)
        {
            _baseValue = baseValue;
            mods = new List<MyMod>();
        }

    }
    public class MyMod
    {
        public ModType mod;
        public int bonus;
        public float duration;
    }


}


