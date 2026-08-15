using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;


[System.Serializable]
public class PassDataContainer
{
    public Brain myBrain;
    public bool canBeBlocked;
    public bool canBeDodged;
    public PassData[] data;
}

[System.Serializable]
public class PassData
{
    public enum HitEffectOnSpell { Nullify, Reflect }
    [ReadOnly] public int priority;

}


[System.Serializable]
public class PassDataManaShield : PassData
{
    public int manaShieldPoints;

    public PassDataManaShield(int manaShieldPoints)
    {
        priority = 20;
        this.manaShieldPoints = manaShieldPoints;
    }
}

[System.Serializable]
public class PassDataKnockBack : PassData
{
    [FormerlySerializedAs("knockBack")] public int knockBackPower;
    public Vector2 knockBackDirection; //ignored if = Vector2.zero

    public PassDataKnockBack(int knockBackPower, Vector2 knockBackDirection = new Vector2())
    {
        priority = 30;
        this.knockBackPower = knockBackPower;
        this.knockBackDirection = knockBackDirection;
    }
}
[System.Serializable]
public class PassDataMagnet : PassData
{
    [InfoBox("If power is greater than knockback resist, magnet is on. Negative value of power turns it off (no comparison to knockback resist.")]
    public int power;
    public Vector3 center;

    public PassDataMagnet(int magnetPower, Vector3 center)
    {
        priority = 40;
        this.power = magnetPower;
        this.center = center;
    }
}
[System.Serializable]
public class PassDataDamage : PassData
{
    public MyDuo<Element, float> pair; //can't serialize dictionary in inspector
    public bool addSpellVelocity; //damage is affected by motion of spell/target. used in 'spiked wall' trap.
    [HideInInspector] public Vector3 spellsVelocity; 

    public PassDataDamage(Element[] keys, float[] values, bool addVelocity = false)
    {
        pair = new MyDuo<Element, float>(keys, values);
        addSpellVelocity = addVelocity;
        priority = 50;
    }
}


[System.Serializable]
public class PassDataSpell: PassData
{
    public SpellMain[] spellsToAffect;
    public HitEffectOnSpell effect;

    public PassDataSpell(SpellMain[] spellsToAffect, HitEffectOnSpell effect)
    {
        priority = 100;
        this.spellsToAffect = spellsToAffect;
        this.effect = effect;
    }
}

[System.Serializable]
public class PassDataStats : PassData 
{
    public Stats stat;
    public GenChange change;
    public int value;
    bool ShowHasDuration() => change == GenChange.Add;
    bool ShowHDuration() => change == GenChange.Add && hasDuration;
    [ShowIf(nameof(ShowHasDuration))] public bool hasDuration;
    [ShowIf(nameof(ShowHDuration))] public float duration;
    
    public PassDataStats(Stats stat, GenChange change, int value, float duration = float.PositiveInfinity)
    {
        priority = 50;
        this.stat = stat;
        this.change = change;
        this.value = value;
        hasDuration = !float.IsPositiveInfinity(duration);
    }
}
// [System.Serializable]
// public class PassDataTag: PassData
// {
//     public string key;
//     public Dictionary<string, string> tags = new Dictionary<string, string>();
//     public const string TagExecutioner = "Executioner";
//     public const string TagStatusBleed = "Bleeding";
//     public const string TagStatusPoison = "Poisoned";
//     public const string TagStatusBurn = "Burning";
//     public const string TagStatusFreeze = "Freezing";
//     public const string TagStatusJolt = "Jolted";
//
// }

