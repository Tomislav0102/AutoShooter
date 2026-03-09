using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyCombat : Combat
{
    protected E_Loco enLoco;
    
    [SerializeField] float projectileSpeed;
    public override Transform MyTarget
    {
        set
        {
            base.MyTarget = value;
            enLoco?.TargetRelay(value == null ? -1 : rangeMelee);
        }
    }

    public override void Initialize(Brain brain)
    {
        base.Initialize(brain);
        enLoco = brain.loco as E_Loco;
        IsReady = true;
    }


    void Update()
    {
        if (MyTarget == null) return;
        float distance = Utils.Distance(br.loco.myTransform.position, MyTarget.position);
        if (distance > rangeRanged)
        {
            br.loco.AttInputEnemy(false);
            br.loco.Att1InputEnemy(false);
        }
        else if (distance >= rangeMelee)
        {
            br.loco.AttInputEnemy(false);
            br.loco.Att1InputEnemy(true);
        }
        else
        {
            br.loco.AttInputEnemy(true);
            br.loco.Att1InputEnemy(false);
        }
      //  if (faceTarget) br.loco.myTransform.LookAt(new Vector3(MyTarget.position.x, br.loco.myTransform.position.y, MyTarget.position.z));
    }


    protected void SpawnProjectile(Transform spawnPointTransform)
    {
        SpawnProjectile(spawnPointTransform.position, spawnPointTransform.rotation);
    }

    protected void SpawnProjectile(Vector3 pos, Quaternion rot)
    {
        E_Projectile projectile = Instantiate(gm.projectilePrefabEnemy, pos, rot) as E_Projectile;
        ProjectilePassData passData = new ProjectilePassData((string st) =>
        {
            print(st);
        }, br, damage, projectileSpeed);
        projectile.InitializeMe(passData);
    }

}
