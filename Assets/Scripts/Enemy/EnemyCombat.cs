using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyCombat : Combat
{
    [SerializeField] GameObject weaponMelee, weaponRanged;
    Spell _spellMelee, _spellRange;

    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            enLoco = value.loco as E_Loco;
            if (weaponMelee != null) _spellMelee = weaponMelee.GetComponent<Spell>();
            if (weaponRanged != null) _spellRange = weaponRanged.GetComponent<Spell>();
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

        if (weaponRanged == null)
        {
            if (weaponMelee == null)
            {
                enLoco.ra = E_Loco.RangeArea.OutOfRange;
                return;
            }
            
            if (distance <= _spellMelee.attackRange) enLoco.ra = E_Loco.RangeArea.Melee;
            else enLoco.ra = E_Loco.RangeArea.OutOfRange;
        }
        else
        {
            if (distance > _spellRange.attackRange) enLoco.ra = E_Loco.RangeArea.OutOfRange;
            else if (weaponMelee != null)
            {
                if (distance <= _spellMelee.attackRange) enLoco.ra = E_Loco.RangeArea.Melee;
                else enLoco.ra = E_Loco.RangeArea.Ranged;
            }
            else enLoco.ra = E_Loco.RangeArea.Ranged;
        }
    }
}
