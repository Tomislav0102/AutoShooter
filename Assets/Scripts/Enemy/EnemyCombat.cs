using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyCombat : Combat
{
    #region RANGES
    /// <summary>
    /// This is only used to switch between weapons in regard to distance to target.
    /// Same applies to spells, some are melee ranged (touch spells), others are like ranged weapons
    /// </summary>
    /// <returns></returns>
    bool Mel() => meleeWeapon != null;
    bool Ran() => rangedWeapon != null;
    [SerializeField] GameObject meleeWeapon;
    [ShowIf(nameof(Mel))]
    [SerializeField] float rangeMelee;
    [SerializeField] GameObject rangedWeapon;
    [ShowIf(nameof(Ran))]
    [SerializeField] float rangeRanged;
    #endregion

    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            enLoco = value.loco as E_Loco;
            IsReady = true;
        }
    }

    protected E_Loco enLoco;
    public override Transform MyTarget
    {
        set
        {
            base.MyTarget = value;
            enLoco.moveCurrent = value == null ? enLoco.moveIdlingDefault : enLoco.moveFightingDefault;
        }
    }


    protected override void Update()
    {
        base.Update();
        if (distance < 0) return; //MyTarget is null

        if (rangedWeapon == null)
        {
            if (meleeWeapon == null)
            {
                enLoco.ra = E_Loco.RangeArea.OutOfRange;
                return;
            }
            
            if (distance <= rangeMelee) enLoco.ra = E_Loco.RangeArea.Melee;
            else enLoco.ra = E_Loco.RangeArea.OutOfRange;
        }
        else
        {
            if (distance > rangeRanged) enLoco.ra = E_Loco.RangeArea.OutOfRange;
            else if (meleeWeapon != null)
            {
                if (distance <= rangeMelee) enLoco.ra = E_Loco.RangeArea.Melee;
                else enLoco.ra = E_Loco.RangeArea.Ranged;
            }
            else enLoco.ra = E_Loco.RangeArea.Ranged;
        }
    }
}
