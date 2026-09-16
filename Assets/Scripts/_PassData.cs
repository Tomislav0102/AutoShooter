using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

#region OLD

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

    /// <summary>
    /// to be removed in future and replaced with data from SoSkill
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="values"></param>
    /// <param name="addVelocity"></param>
    public PassDataDamage(Element[] keys, float[] values, bool addVelocity = false)
    {
        pair = new MyDuo<Element, float>(keys, values);
        addSpellVelocity = addVelocity;
    }
    /// <summary>
    /// this will be the only constructor
    /// </summary>
    /// <param name="pair"></param>
    /// <param name="addVelocity"></param>
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
        public GenChange change;
        bool ShowBuffType() => change == GenChange.Add;
        [ShowIf(nameof(ShowBuffType))] public BuffType buffType;
        public Buff buff;
        
        public Group(GenChange change, BuffType buffType, Buff buff) 
        {
            this.change = change;
            this.buffType = buffType;
            this.buff = buff;
        }
        public Group(GenChange change, Buff buff) //for removal
        {
            this.change = change;
            this.buff = buff;
        }
    }
}

[System.Serializable]
public class PassDataEffect: PassData
{
    public EffectGroup[] group;
}
#endregion


[System.Serializable]
public class TheBlock
{
    public enum HitEffectOnSpell { Nullify, Reflect }
    public Brain myBrain;
    [BoxGroup] public bool canBeBlocked;
    [BoxGroup] public bool canBeDodged;

    public bool hasDamage;
    [ShowIf(nameof(hasDamage))] public MyDuo<Element, float> damagePair;
    [HideInInspector] public Vector2 spellsVelocity; //ignored if = Vector2.zero
    
    public bool hasKnockback;
    [ShowIf(nameof(hasKnockback))] public int knockbackPower;
    [ShowIf(nameof(hasKnockback))] public Vector2 knockbackDirection; //ignored if = Vector2.zero
    
    public bool hasDash;
    [ShowIf(nameof(hasDash))] public int dashPower;
    [ShowIf(nameof(hasDash))] public Vector2 dashDirection; 
    
    public bool hasManaShield;
    [ShowIf(nameof(hasManaShield))] public int manaShieldPoints;
    
    public bool hasSpell;
    [ShowIf(nameof(hasSpell))] public MyDuo<HitEffectOnSpell, SpellMain> spellPair;
    
    public bool hasStats;
    [ShowIf(nameof(hasStats))] public StatsGroup[] stats;
    
    public bool hasEffect;
    [ShowIf(nameof(hasEffect))] public EffectGroup[] effects;
}

[System.Serializable]
public class StatsGroup
{
    public GenChange change;
    bool ShowBuffType() => change == GenChange.Add;
    [ShowIf(nameof(ShowBuffType))] public BuffType buffType;
    public Buff buff;
        
    public StatsGroup(GenChange change, BuffType buffType, Buff buff) 
    {
        this.change = change;
        this.buffType = buffType;
        this.buff = buff;
    }
    public StatsGroup(GenChange change, Buff buff) //for removal
    {
        this.change = change;
        this.buff = buff;
    }
}

[System.Serializable]
public class EffectGroup
{
    public Brain brain; //redundant
    public Status.Effect effect;
    public float duration;
    public int intensity;

    public EffectGroup(Brain brain, Status.Effect effect, float duration = float.PositiveInfinity, int intensity = 0)
    {
        this.brain = brain;
        this.effect = effect;
        this.duration = duration;
        this.intensity = intensity;
    }
        
    public bool IsDot() => effect == Status.Effect.Bleeding || 
                           effect == Status.Effect.Burning || 
                           effect == Status.Effect.Freezing || 
                           effect == Status.Effect.Jolted ||  
                           effect == Status.Effect.Poisoned;
}




