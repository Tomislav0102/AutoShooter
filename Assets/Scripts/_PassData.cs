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
    [FormerlySerializedAs("knockBackPower")] public int power;
    [FormerlySerializedAs("knockBackDirection")] public Vector2 direction; //ignored if = Vector2.zero

    public PassDataKnockBack(int power, Vector2 direction = new Vector2())
    {
        priority = 30;
        this.power = power;
        this.direction = direction;
    }
}
public class PassDataDash : PassData
{
    public int power;
    public Vector2 direction; //if Vector2.zero then use Br.MyTransform.forward
    
    public PassDataDash(int power, Vector2 direction = new Vector2())
    {
        priority = 25;
        this.power = power;
        this.direction = direction;
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
    bool ShowDuration() => change == GenChange.Add && hasDuration;
    [ShowIf(nameof(ShowHasDuration))] public bool hasDuration;
    [ShowIf(nameof(ShowDuration))] public float duration;
    
    public PassDataStats(Stats stat, GenChange change, int value, float duration = float.PositiveInfinity)
    {
        priority = 50;
        this.stat = stat;
        this.change = change;
        this.value = value;
        hasDuration = !float.IsPositiveInfinity(duration);
    }
}
[System.Serializable]
public class PassDataEffect: PassData
{
    public EffectGroup[] group;
    
    
    [System.Serializable]
    public struct EffectGroup
    {
        public AttackEffect effect;
        public float duration;
        public int damagePerTick;
    }
}

