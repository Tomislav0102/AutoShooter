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
            buffs = new List<BuffStats>();
            ResetFinalStats();
        }
    }
    Brain _br;
    public Dictionary<Stats, float> statsFinal = new Dictionary<Stats, float>();
    public List<BuffStats> buffs = new List<BuffStats>();


    #region GET STATS
    
    public int GetStat(Stats stat)
    {
        return (int)statsFinal[stat];
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

    Stats StatByElement(Element element, bool isDamage = true)
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


    public void BuffInjectData(StatsGroup group)
    {
        BuffStats buffStats = group.buffStats;
        switch (group.change)
        {
            case GenChange.Add:
                if (group.buffType == BuffType.Percentage && Mathf.Approximately(buffStats.data.value, 1f))
                {
                    if (Br.debug) print("Buff multiplier is 1X, so its ignored");
                    return;
                }
                int previousValue = GetStat(buffStats.stat);
                float finalValue = buffStats.data.value;
                switch (group.buffType)
                {
                    case BuffType.Added:
                        statsFinal[buffStats.stat] += finalValue;
                        break;
                    case BuffType.Percentage:
                        finalValue = data.baseStats[buffStats.stat] * (buffStats.data.value - 1f);
                        statsFinal[buffStats.stat] += finalValue;
                        break;
                }
                buffs.Add(buffStats);
                if (Br.debug) print($"{buffStats.stat} changed from {previousValue} to {GetStat(buffStats.stat)}");
                break;
            case GenChange.Remove:
                RemoveBuff(buffStats);
                break;
        }
    }

    void Update()
    {
        foreach (BuffStats item in buffs)
        {
            if (item.data.permanent || float.IsPositiveInfinity(item.data.Duration)) continue;
            if (item.data.Duration > 0)
            {
               // item.Duration -= Time.deltaTime;
                continue;
            }
            RemoveBuff(item);
        }
    }
    void RemoveBuff(BuffStats buffStatsToRemove)
    {
        if (buffStatsToRemove == null || !buffs.Contains(buffStatsToRemove)) return;
        statsFinal[buffStatsToRemove.stat] -= buffStatsToRemove.data.value;
        buffs.Remove(buffStatsToRemove);
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


