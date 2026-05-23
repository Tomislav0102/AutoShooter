using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;


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
    Dictionary<SkillReq, int> _requirements;
    public System.Action<SkillReq> onSkillReqValidated;
    public int GetStat(Stats stat) => _myStats[(int)stat].Value;


    public void ProcessSkillReqIncrease(SkillReq skill)
    {
        if (_requirements == null || _requirements.Count == 0)
        {
            _requirements = new Dictionary<SkillReq, int>(); 
            int length = System.Enum.GetNames(typeof(SkillReq)).Length;     
            for (int i = 0; i < length; i++)
            {
                _requirements.Add((SkillReq)i, 0);
            }
        }
        if (skill == SkillReq.Hit || skill == SkillReq.Miss) Continuation(SkillReq.Strike);
        Continuation(skill);
        return;
        
        void Continuation(SkillReq skillReq)
        {
            _requirements[skillReq]++;
            if (data.skillRequirements[skillReq] > 0 && _requirements[skillReq] % data.skillRequirements[skillReq] == 0) onSkillReqValidated?.Invoke(skillReq);
        }
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
    }


}


