using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


/// <summary>
/// used in 'Reflect projectiles' and many traps
/// </summary>
public class OvrPassData : MonoBehaviour
{
    public PassDataBlock block;
}


[System.Serializable]
public class PassDataBlock
{
    [BoxGroup] public bool canBeBlocked;
    [BoxGroup] public bool canBeDodged;

    [SerializeField] bool hasDamage;
    [ShowIf(nameof(hasDamage))] public PassDataDamage damage;
    [SerializeField] bool hasKnockback;
    [ShowIf(nameof(hasKnockback))] public PassDataKnockBack knockBack;
    [SerializeField] bool hasManaShield;
    [ShowIf(nameof(hasManaShield))] public PassDataManaShield manaShield;
    [SerializeField] bool hasSpell;
    [ShowIf(nameof(hasSpell))] public PassDataSpell spellData;
    [SerializeField] bool hasStats;
    [ShowIf(nameof(hasStats))] public PassDataStats stats;

    
    public PassDataContainer GetContainer()
    {
        PassDataContainer container = new PassDataContainer()
        {
            canBeBlocked = this.canBeBlocked,
            canBeDodged = this.canBeDodged,
        };
        List<PassData> data = new List<PassData>();   
        if (hasDamage && damage.pair.Length() > 0) data.Add(damage);
        if (hasKnockback && knockBack.power > 0) data.Add(knockBack);
        if (hasManaShield && manaShield.manaShieldPoints > 0) data.Add(manaShield);
        if (hasSpell) data.Add(spellData);
        if (hasStats) data.Add(stats);
        
        container.data = data;
        
        return container;
    }

}

