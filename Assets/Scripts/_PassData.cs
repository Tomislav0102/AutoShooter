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
    public List<PassData> data;

    public PassDataContainer()
    {
        data = new List<PassData>();
    }
}

[System.Serializable]
public class PassData
{
    public enum HitEffectOnSpell { Nullify, Reflect }
}


[System.Serializable]
public class PassDataManaShield : PassData
{
    public int manaShieldPoints;

    public PassDataManaShield(int manaShieldPoints)
    {
        this.manaShieldPoints = manaShieldPoints;
    }
}

[System.Serializable]
public class PassDataKnockBack : PassData
{
    public int power;
    public Vector2 direction; //ignored if = Vector2.zero

    public PassDataKnockBack(int power, Vector2 direction = new Vector2())
    {
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
    }
    public PassDataDamage(MyDuo<Element, float> pair, bool addVelocity = false)
    {
        this.pair = pair;
        addSpellVelocity = addVelocity;
    }
}


[System.Serializable]
public class PassDataSpell: PassData
{
    public MyDuo<HitEffectOnSpell, SpellMain> pair;
    public PassDataSpell(HitEffectOnSpell[] keys, SpellMain[] values)
    {
        pair = new MyDuo<HitEffectOnSpell, SpellMain>(keys, values);
    }
    public PassDataSpell(MyDuo<HitEffectOnSpell, SpellMain> pair)
    {
        this.pair = pair;
    }

}

[System.Serializable]
public class PassDataStats : PassData 
{
    public Group[] group;
    
    [System.Serializable]
    public class Group
    {
        public Stats stat;
        public GenChange change;
        public int value;
        bool ShowHasDuration() => change == GenChange.Add;
        bool ShowDuration() => change == GenChange.Add && hasDuration;
        [ShowIf(nameof(ShowHasDuration))] public bool hasDuration;
        [ShowIf(nameof(ShowDuration))] public float duration;
        
        public Group(Stats stat, GenChange change, int value, float duration = float.PositiveInfinity)
        {
            this.stat = stat;
            this.change = change;
            this.value = value;
            hasDuration = !float.IsPositiveInfinity(duration);
        }
    }
}
[System.Serializable]
public class PassDataEffect: PassData
{
    public Group[] group;
    
    [System.Serializable]
    public class Group
    {
        public AttackEffect effect;
        public float duration;
        public int damagePerTick;

        public Group(AttackEffect effect, float duration = 0, int damagePerTick = 1)
        {
            this.effect = effect;
            this.duration = duration;
            this.damagePerTick = damagePerTick;
        }
    }
}

