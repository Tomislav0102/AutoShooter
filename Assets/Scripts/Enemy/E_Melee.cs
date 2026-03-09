using System;
using UnityEngine;

public class E_Melee : EnemyCombat
{
    AttackOverlapSphere _sphere;

    public override void Initialize(Brain brain)
    {
        base.Initialize(brain);
        _sphere = new AttackOverlapSphere(brain.loco.myTransform, rangeMelee, Utils.IsInLayerMask(gameObject, gm.layPlayer) ? gm.layEnemies : gm.layPlayer);
    }

    public override void AE_Attack(int num = 0)
    {
        base.AE_Attack(num);
        switch (num)
        {
            case 0: //melee
                _sphere.Attack(damage);
                break;
            case 1: //projectile
                SpawnProjectile(br.loco.myTransform.position + Vector3.up, br.loco.myTransform.rotation);
                break;
        }
    }

    

}