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
            _eLoco = Br.loco as E_Loco;
            IsInitialized = true;
        }
    }
    E_Loco _eLoco;

    public override Transform MyTarget
    {
        set
        {
            base.MyTarget = value;
            if (value == null)
            {
                _eLoco.moveCurrent = _eLoco.moveIdlingDefault;
                _eLoco.AttInputEnemy(false);
                _eLoco.Att1InputEnemy(false);

                _eLoco.ra = E_Loco.RangeArea.OutOfRange;
            }
            else
            {
                _eLoco.moveCurrent = _eLoco.moveFightingDefault;
                
                if (rangedWeapon == null)
                {
                    if (meleeWeapon == null)
                    {
                        _eLoco.ra = E_Loco.RangeArea.OutOfRange;
                        return;
                    }
            
                    if (distanceToTarget <= rangeMelee) _eLoco.ra = E_Loco.RangeArea.Melee;
                    else _eLoco.ra = E_Loco.RangeArea.OutOfRange;
                }
                else
                {
                    if (distanceToTarget > rangeRanged) _eLoco.ra = E_Loco.RangeArea.OutOfRange;
                    else if (meleeWeapon != null)
                    {
                        if (distanceToTarget <= rangeMelee) _eLoco.ra = E_Loco.RangeArea.Melee;
                        else _eLoco.ra = E_Loco.RangeArea.Ranged;
                    }
                    else _eLoco.ra = E_Loco.RangeArea.Ranged;
                }

            }
        }
    }



}


