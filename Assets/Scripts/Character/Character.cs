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
    [HideInInspector] public System.Action<Stats, float> onStatChange;
    public SoCharacter statsBase;
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            if (statsBase == null) statsBase = Ga.me.defCharacter;
            _buffTimers = new MyDuo<BuffStats, float>();
            ResetFinalStats();
            value.onAllChildrenInitialized += () =>
            {
                int statsLength = System.Enum.GetNames(typeof(Stats)).Length;
                for (int i = 0; i < statsLength; i++)
                {
                    onStatChange?.Invoke(_statsFinal.GetKey(i), _statsFinal.GetValue(i));
                }
            };
            bodySizeEvent();
            return;
            
            void bodySizeEvent()
            {
                _dataBodySizeChange = new MyDuo<Stats, float>();//Stat += float * (Stat-100)
                _dataBodySizeChange.Add(Stats.DamPhysical, 0.5f);
                _dataBodySizeChange.Add(Stats.ResistPhysical, 0.5f);
                _dataBodySizeChange.Add(Stats.Dodge, -1f);
                _dataBodySizeChange.Add(Stats.MoveSpeed, -0.2f);
                _dataBodySizeChange.Add(Stats.AttackSpeed, -0.15f);
                _dataBodySizeChange.Add(Stats.CritChance, -1f);
                _dataBodySizeChange.Add(Stats.Size, 2f);
                _dataBodySizeChange.Add(Stats.KnockBack, 0.1f);
                _dataBodySizeChange.Add(Stats.Health, 0.5f);
                _dataBodySizeChange.Add(Stats.SightRange, 0.1f);
                onStatChange += (Stats st, float val) =>
                {
                    switch (st)
                    {
                        case Stats.BodySize:
                            for (int i = 0; i < _buffStatsBodySizeChange.Length; i++)
                            {
                                CharacterInjectData(GenChange.Remove, _buffStatsBodySizeChange[i]);
                            }
                            _buffStatsBodySizeChange = new BuffStats[_dataBodySizeChange.Length()];
                            for (int i = 0; i < _dataBodySizeChange.Length(); i++)
                            {
                                float fl = _dataBodySizeChange.GetValue(i) * (val - 100);
                                _buffStatsBodySizeChange[i] = new BuffStats(_dataBodySizeChange.GetKey(i), BuffType.Added, fl);
                                CharacterInjectData(GenChange.Add, _buffStatsBodySizeChange[i]);
                            }
                            break;
                    }
                };
            }
            }

    }
    Brain _br;
    public MyDuo<Stats, float> _statsFinal = new MyDuo<Stats, float>();
    MyDuo<Stats, float> _ovrStatsFinal = new MyDuo<Stats, float>(); //for BuffType.Set
    MyDuo<BuffStats, float> _buffTimers = new MyDuo<BuffStats, float>();
    BuffStats[] _buffStatsBodySizeChange = System.Array.Empty<BuffStats>();
    MyDuo<Stats, float> _dataBodySizeChange = new MyDuo<Stats, float>();
    
    void Update()
    {
        for (int i = 0; i < _buffTimers.Length(); i++)
        {
            BuffStats buff = _buffTimers.GetKey(i);
            if (buff.data.permanent || float.IsPositiveInfinity(buff.data.Duration)) continue;
            float duration = _buffTimers.GetValue(i);
            if (duration > 0)
            {
                duration -= Time.deltaTime;
                _buffTimers.SetValue(i, duration);
                continue;
            }
            RemoveBuff(buff);
        }
    }


    #region GET STATS

    public int GetStat(Stats stat)
    {
        if (_ovrStatsFinal.HasKey(stat)) return (int)_ovrStatsFinal.GetValueByKey(stat);
        return (int)_statsFinal.GetValueByKey(stat);
    }

    public MyDuo<Element, float> GetDamage(Element element = Element.Physical, float multiplier = 1f, MyDuo<Element, float> extraDamage = null)
    {
        if (extraDamage == null) return new MyDuo<Element, float>(new Element[1] { element }, new float[1] { GetStat(StatByElement(element)) * multiplier });
        
        MyDuo<Element, float> damage = new MyDuo<Element, float>();
        damage.Add(element, (GetStat(StatByElement(element)) + ExtraDamageValue(element, extraDamage)) * multiplier);
        for (int i = 0; i < extraDamage.Length(); i++)
        {
            if (extraDamage.GetKey(i) == element) continue;
            damage.Add(extraDamage.GetKey(i), (GetStat(StatByElement(extraDamage.GetKey(i))) + extraDamage.GetValue(i)) * multiplier);
        }

        return damage;
    }
    public MyDuo<Element, float> GetDamage(Element[] elements, float multiplier = 1f, MyDuo<Element, float> extraDamage = null)
    {
        MyDuo<Element, float> damage = new MyDuo<Element, float>();
        foreach (Element el in elements)
        {
            damage.Add(el, (GetStat(StatByElement(el)) + ExtraDamageValue(el, extraDamage)) * multiplier);
        }
        
        return damage;
    }

    public static Stats StatByElement(Element element, bool isDamage = true)
    {
        switch (element)
        {
            case Element.Fire:
                return isDamage ? Stats.DamFire : Stats.ResistFire;
            case Element.Ice:
                return isDamage ? Stats.DamIce : Stats.ResistIce;
            case Element.Electricity:
                return isDamage ? Stats.DamElectricity : Stats.ResistElectricity;
            case Element.Poison:
                return isDamage ? Stats.DamPoison : Stats.ResistPoison;
            case Element.Magic:
                return isDamage ? Stats.DamMagic : Stats.ResistMagic;
            default:
                return isDamage ? Stats.DamPhysical : Stats.ResistPhysical;
        }
    }
    float ExtraDamageValue(Element el, MyDuo<Element, float> extraDamage)
    {
        if (extraDamage != null && extraDamage.TryGetValueByKey(el, out float val)) return val;
        return 0f;
    }
    #endregion

    
    public void CharacterInjectData(GenChange change, BuffStats buffStats)
    {
        if (buffStats is null) return;
        switch (change)
        {
            case GenChange.Add:
                if (buffStats.buffType == BuffType.Percentage && Mathf.Approximately(buffStats.data.value, 1f))
                {
                    if (Br.debug) print("Buff multiplier is 1X, so its ignored");
                    return;
                }
                if (buffStats.buffType == BuffType.Added && Mathf.Approximately(buffStats.data.value, 0f))
                {
                    if (Br.debug) print("Buff addition is 0, so its ignored");
                    return;
                }
                Stats stat = buffStats.stat;
                int previousValueDebug = GetStat(stat);
                float finalValue = buffStats.data.value;
                switch (buffStats.buffType)
                {
                    case BuffType.Added:
                        finalValue += _statsFinal.GetValueByKey(stat);
                        _statsFinal.SetValueByKey(stat, finalValue);
                        break;
                    case BuffType.Percentage:
                        finalValue = statsBase.baseStats[stat] * (buffStats.data.value * 0.01f);
                        finalValue += _statsFinal.GetValueByKey(stat);
                        _statsFinal.SetValueByKey(stat, finalValue);
                        break;
                    case BuffType.Set:
                        if (_ovrStatsFinal.HasKey(stat)) _ovrStatsFinal.SetValueByKey(stat, finalValue);
                        else _ovrStatsFinal.Add(stat, finalValue);
                        break;
                }
                onStatChange?.Invoke(buffStats.stat, finalValue);
                _buffTimers.Add(buffStats, buffStats.data.Duration);
                if (Br.debug) print($"{stat} changed from {previousValueDebug} to {GetStat(stat)}");
                break;
            case GenChange.Remove:
                RemoveBuff(buffStats);
                break;
        }
    }

    void RemoveBuff(BuffStats buffToRemove)
    {
        if (!_buffTimers.HasKey(buffToRemove)) return;
        switch (buffToRemove.buffType)
        {
            case BuffType.Added:
                float valAdded = _statsFinal.GetValueByKey(buffToRemove.stat) - buffToRemove.data.value;
                _statsFinal.SetValueByKey(buffToRemove.stat, valAdded);
                break;
            case BuffType.Percentage:
                float valPercentage = _statsFinal.GetValueByKey(buffToRemove.stat) - statsBase.baseStats[buffToRemove.stat] * (buffToRemove.data.value * 0.01f);
                _statsFinal.SetValueByKey(buffToRemove.stat, valPercentage);
                break;
            case BuffType.Set:
                if (!_ovrStatsFinal.HasKey(buffToRemove.stat)) return;  
                _ovrStatsFinal.RemoveByKey(buffToRemove.stat);
                break;
        }
        onStatChange?.Invoke(buffToRemove.stat, GetStat(buffToRemove.stat));
        _buffTimers.RemoveByKey(buffToRemove);
        //to mitigate problem of float precision (baseStats are integers, while finalStats are floats)
        if (_buffTimers.Length() == 0) ResetFinalStats(); 

    }
    void ResetFinalStats()
    {
        _statsFinal = new MyDuo<Stats, float>();
        foreach (KeyValuePair<Stats, int> item in statsBase.baseStats)
        {
            _statsFinal.Add(item.Key, item.Value);
        }
    }
    

}


