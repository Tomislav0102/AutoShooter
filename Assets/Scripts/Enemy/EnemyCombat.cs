using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyCombat : Combat
{
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
        if (distance > holster.spells[GenOrder.Secondary].myData.attackRange)
        {
            enLoco.ra = E_Loco.RangeArea.OutOfRange;
        }
        else if (distance > holster.spells[GenOrder.Primary].myData.attackRange)
        {
            enLoco.ra = E_Loco.RangeArea.Ranged;
        }
        else
        {
            enLoco.ra = E_Loco.RangeArea.Melee;
        }
    }
}
