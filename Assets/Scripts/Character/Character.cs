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
    [HideInInspector] public System.Action<Stats> onStatChange;
    [SerializeField] SoCharacter statsBase;
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            if (statsBase == null) statsBase = Ga.me.defCharacter;
            _buffTimers = new MyDuo<BuffStats, float>();
            ResetFinalStats();
        }
    }
    Brain _br;
    MyDuo<Stats, float> _statsFinal = new MyDuo<Stats, float>();
    MyDuo<Stats, float> _ovrStatsFinal = new MyDuo<Stats, float>(); //for BuffType.Set
    MyDuo<BuffStats, float> _buffTimers = new MyDuo<BuffStats, float>();
    
    
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

    
    public void CharacterInjectData(GenChange change, BuffStats buffStats)
    {
        switch (change)
        {
            case GenChange.Add:
                if (buffStats.buffType == BuffType.Percentage && Mathf.Approximately(buffStats.data.value, 1f))
                {
                    if (Br.debug) print("Buff multiplier is 1X, so its ignored");
                    return;
                }
                Stats stat = buffStats.stat;
                int previousValueDebug = GetStat(stat);
                float finalValue = buffStats.data.value;
                switch (buffStats.buffType)
                {
                    case BuffType.Added:
                        float valAdded = _statsFinal.GetValueByKey(stat) + finalValue;
                        _statsFinal.SetValueByKey(stat, valAdded);
                        break;
                    case BuffType.Percentage:
                        finalValue = statsBase.baseStats[stat] * (buffStats.data.value - 1f);
                        float valPercentage = _statsFinal.GetValueByKey(stat) + finalValue;
                        _statsFinal.SetValueByKey(stat, valPercentage);
                        break;
                    case BuffType.Set:
                        if (_ovrStatsFinal.HasKey(stat)) _ovrStatsFinal.SetValueByKey(stat, finalValue);
                        else _ovrStatsFinal.Add(stat, finalValue);
                        break;
                }
                if (buffStats.data.value < 0) //debuffs
                {
                    switch (stat)
                    {
                        case Stats.ResistPhysical:
                            Instantiate(Ga.me.psArmorBreak, Br.myTransform.position + 1.5f * Vector3.up, Quaternion.identity, Ga.me.spells.myTransform);
                            FloatingText ft = Instantiate(Ga.me.uiManager.floatingTextPrefab, Br.myTransform.position, Quaternion.identity, Ga.me.uiManager.floatingContainer);
                            ft.SpawnMe("Armor broken", Color.darkOrchid);
                            break;
                    }
                }
                onStatChange?.Invoke(buffStats.stat);
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
        if (buffToRemove is null || !_buffTimers.HasKey(buffToRemove)) return;
        switch (buffToRemove.buffType)
        {
            case BuffType.Added:
                float valAdded = _statsFinal.GetValueByKey(buffToRemove.stat) - buffToRemove.data.value;
                _statsFinal.SetValueByKey(buffToRemove.stat, valAdded);
                break;
            case BuffType.Percentage:
                float valPercentage = _statsFinal.GetValueByKey(buffToRemove.stat) - statsBase.baseStats[buffToRemove.stat] * (buffToRemove.data.value - 1f);
                _statsFinal.SetValueByKey(buffToRemove.stat, valPercentage);
                break;
            case BuffType.Set:
                if (!_ovrStatsFinal.HasKey(buffToRemove.stat)) return;  
                _ovrStatsFinal.Remove(buffToRemove.stat);
                break;
        }
        onStatChange?.Invoke(buffToRemove.stat);
        _buffTimers.Remove(buffToRemove);
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


