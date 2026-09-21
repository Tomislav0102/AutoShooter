using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


/// <summary>
/// used in 'Reflect projectiles' and many traps
/// </summary>
public class OvrPassData : MonoBehaviour
{
    public PassData passData;
}

[System.Serializable]
public class PassData
{
    [ReadOnly] public Brain myBrain;
    [BoxGroup] public bool canBeBlocked;
    [BoxGroup] public bool canBeDodged;

    public bool hasDamage;
    [ShowIf(nameof(hasDamage))] public MyDuo<Element, float> damagePair;
    //not working, maybe needs bool hasSpellVelocity
    [HideInInspector] public Vector2 spellsVelocity; //ignored if = Vector2.zero
    
    public bool hasKnockback;
    [ShowIf(nameof(hasKnockback))] public int knockbackPower;
    [ShowIf(nameof(hasKnockback))] public Vector2 knockbackDirection; //ignored if = Vector2.zero
    
    public bool hasDash;
    [ShowIf(nameof(hasDash))] public int dashPower;
    [ShowIf(nameof(hasDash))] public Vector2 dashDirection; 
    
    public bool hasManaShield;
    [ShowIf(nameof(hasManaShield))] public int manaShieldPoints;

    #region ToBeRemoved
    public bool hasSpell;
    [ShowIf(nameof(hasSpell))] public MyDuo<SpellMain.HitEffectOnSpell, SpellMain> spellPair;
    #endregion 

    public bool hasStats;
    [ShowIf(nameof(hasStats))] public StatsGroup[] stats;
    
    public bool hasEffect;
    [ShowIf(nameof(hasEffect))] public BuffEffects[] effects;
}



