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
            buffs = new List<Buff>();
            ResetFinalStats();
        }
    }
    Brain _br;
    public Dictionary<Stats, float> statsFinal = new Dictionary<Stats, float>();
    public List<Buff> buffs = new List<Buff>();


    public int GetStat(Stats stat)
    {
        return (int)statsFinal[stat];
    }
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
    public MyDuo<Element, float> GetDamage(Stats statOffensive, MyDuo<Element, float> extraDamage = null)
    {
        MyDuo<Element, float> damage = GetElDamage();
        damage.AddRange(extraDamage);
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
                if (Br.debug) print("must use offensive stat");
                return null;
        }

        return damage;
    }


    public void BuffInjectData(StatsGroup group)
    {
        Buff buff = group.buff;
        switch (group.change)
        {
            case GenChange.Add:
                if (group.buffType == BuffType.Percentage && Mathf.Approximately(buff.value, 1f))
                {
                    if (Br.debug) print("Buff multiplier is 1X, so its ignored");
                    return;
                }
                int previousValue = GetStat(buff.stat);
                float finalValue = buff.value;
                switch (group.buffType)
                {
                    case BuffType.Added:
                        statsFinal[buff.stat] += finalValue;
                        break;
                    case BuffType.Percentage:
                        finalValue = data.baseStats[buff.stat] * (buff.value - 1f);
                        statsFinal[buff.stat] += finalValue;
                        break;
                }
                buffs.Add(buff);
                if (Br.debug) print($"{buff.stat} changed from {previousValue} to {GetStat(buff.stat)}");
                break;
            case GenChange.Remove:
                RemoveBuff(buff);
                break;
        }
    }

    void Update()
    {
        foreach (Buff item in buffs)
        {
            if (item.permanent || float.IsPositiveInfinity(item.duration)) continue;
            if (item.duration > 0)
            {
                item.duration -= Time.deltaTime;
                continue;
            }
            RemoveBuff(item);
        }
    }
    void RemoveBuff(Buff buffToRemove)
    {
        if (buffToRemove == null || !buffs.Contains(buffToRemove)) return;
        statsFinal[buffToRemove.stat] -= buffToRemove.value;
        buffs.Remove(buffToRemove);
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
[System.Serializable]
public class Buff
{
    public Stats stat;
    public float value;
    public bool permanent;
    [HideIf(nameof(permanent))] public float duration;

    public Buff() { }
    public Buff(Stats stat, float val, float dur = float.PositiveInfinity)
    {
        this.stat = stat;
        value = val;
        duration = dur;
        permanent = float.IsPositiveInfinity(duration);
    }
}


