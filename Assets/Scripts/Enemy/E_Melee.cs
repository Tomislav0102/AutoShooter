using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class E_Melee : EnemyCombat
{
    [Title("Projectile data")]
    [SerializeField][Range(0, 3)] int ricochet;
    [SerializeField][Range(0, 3)] int pierce, bounce;
    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        switch (num)
        {
            case 0: //melee
                Spell melee = Instantiate(Ga.me.spells.melee,
                    Br.loco.myTransform.position,
                    Quaternion.identity, Ga.me.spells.myTransform).GetComponent<Spell>();
                melee.myTransform.position += melee.attackRange * Br.loco.myTransform.forward;
                melee.InitializeMe(Br);
                break;
            case 1: //projectile
                S_Bullet projectile = Instantiate(Ga.me.spells.projectile, Br.loco.myTransform.position + Vector3.up, Br.loco.myTransform.rotation, Ga.me.spells.myTransform).GetComponent<S_Bullet>();
                projectile.ricochet = ricochet;
                projectile.pierce = pierce;
                projectile.bounce = bounce;
                projectile.InitializeMe(Br);
                break;
        }
    }

    

}