using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyCombat : Combat
{
    [SerializeField] protected Transform spawnPoint;
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
                _eLoco.BehCurrent = _eLoco.behIdlingDefault;
            }
            else
            {
                _eLoco.BehCurrent = _eLoco.behFightingDefault;
            }
        }
    }
}




// public class EnemyCombat : Combat
// {
//     #region RANGES
//     /// <summary>
//     /// This is only used to switch between weapons in regard to distance to target.
//     /// Same applies to spells, some are melee ranged (touch spells), others are like ranged weapons
//     /// </summary>
//     /// <returns></returns>
//     bool Mel() => meleeWeapon != null;
//     public bool Ran() => rangedWeapon != null;
//     [SerializeField] protected SpellMain meleeWeapon;
//     [SerializeField] protected SpellMain rangedWeapon;
//     [ShowIf(nameof(Ran))]
//     [SerializeField] float rangeRanged;
//     #endregion
//     [SerializeField] protected Transform spawnPoint;
//
//     public override Brain Br
//     {
//         get => base.Br;
//         set
//         {
//             base.Br = value;
//             _eLoco = Br.loco as E_Loco;
//             IsInitialized = true;
//         }
//     }
//     E_Loco _eLoco;
//
//     public override Transform MyTarget
//     {
//         set
//         {
//             base.MyTarget = value;
//             if (value == null)
//             {
//                 _eLoco.BehCurrent = _eLoco.behIdlingDefault;
//                 for (int i = 0; i < 2; i++)
//                 {
//                     _eLoco.AttackAnimation(false, i);
//                 }
//                 _eLoco.weaponRange = E_Loco.RangeArea.OutOfRange;
//             }
//             else
//             {
//                 _eLoco.BehCurrent = _eLoco.behFightingDefault;
//                 
//                 if (rangedWeapon == null)
//                 {
//                     if (meleeWeapon == null)
//                     {
//                         _eLoco.weaponRange = E_Loco.RangeArea.OutOfRange;
//                         return;
//                     }
//
//                     if (distanceToTarget <= meleeWeapon.spell.areaOfEffect) _eLoco.weaponRange = E_Loco.RangeArea.Melee;
//                     else _eLoco.weaponRange = E_Loco.RangeArea.OutOfRange;
//                 }
//                 else
//                 {
//                     if (distanceToTarget > rangeRanged)
//                     {
//                         _eLoco.weaponRange = E_Loco.RangeArea.OutOfRange;
//                     }
//                     else if (meleeWeapon != null)
//                     {
//                         if (distanceToTarget <= meleeWeapon.spell.areaOfEffect) _eLoco.weaponRange = E_Loco.RangeArea.Melee;
//                         else _eLoco.weaponRange = E_Loco.RangeArea.Ranged;
//                     }
//                     else _eLoco.weaponRange = E_Loco.RangeArea.Ranged;
//                 }
//     
//             }
//         }
//     }
// }
