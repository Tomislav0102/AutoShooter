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
    [SerializeField] float projectileSpeed;
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
        // if (distance > weapons[1].myData.areaOfEffect)
        // {
        //     enLoco.ra = E_Loco.RangeArea.OutOfRange;
        // }
        // else if (distance > weapons[0].myData.areaOfEffect)
        // {
        //     enLoco.ra = E_Loco.RangeArea.Ranged;
        // }
        // else
        // {
        //     enLoco.ra = E_Loco.RangeArea.Melee;
        // }
    }

    // protected void SpawnProjectile(Vector3 pos, Quaternion rot)
    // {
    //     E_Projectile projectile = Instantiate(GameManager.Instance.projectilePrefabEnemy, pos, rot) as E_Projectile;
    //     ProjectilePassData passData = new ProjectilePassData((string st) =>
    //     {
    //         print(st);
    //     }, br, weapons[1].myData.damage, projectileSpeed);
    //     projectile.InitializeMe(passData);
    // }

}
