using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// used in 'Reflect projectiles' and many traps
/// </summary>
public class OvrPassData : MonoBehaviour
{
    public bool canBeBlocked;

    public PassDataDamage damage;
    public PassDataKnockBack knockBack;
    public PassDataMagnet magnet;
    public PassDataManaShield manaShield;
    public PassDataSpell spellData;
    public PassDataStats stats;

    
    public PassDataContainer GetContainer()
    {
        PassDataContainer container = new PassDataContainer()
        {
            canBeBlocked = canBeBlocked,
        };
        List<PassData> data = new List<PassData>();   
        if (damage.pair.Length() > 0) data.Add(damage);
        if (knockBack.knockBackPower > 0) data.Add(knockBack);
        if (manaShield.manaShieldPoints > 0) data.Add(manaShield);
        if (spellData.effect != PassData.HitEffectOnSpell.None) data.Add(spellData);
        if (stats.change != GenCalcChange.None) data.Add(stats);
        
        container.data = data.ToArray();
        
        return container;
    }
}

