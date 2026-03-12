using System;
using UnityEngine;

public class E_Melee : EnemyCombat
{

    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        switch (num)
        {
            case 0: //melee
               // _sphere.Attack(weapons[0].damage);
                break;
            case 1: //projectile
              //  SpawnProjectile(br.loco.myTransform.position + Vector3.up, br.loco.myTransform.rotation);
                break;
        }
    }

    

}