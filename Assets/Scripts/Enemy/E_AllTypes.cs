using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class E_AllTypes : EnemyCombat
{
    [Title("Projectile data")]
    [SerializeField] Transform spawnPoint;
    [SerializeField][Range(0, 3)] int ricochet;
    [SerializeField][Range(0, 3)] int pierce, bounce;
    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        switch (num)
        {
            case 0: //melee
                Spell melee = Instantiate(Ga.me.spells.meleeEnemy,
                    Br.myTransform.position,
                    Quaternion.identity, Ga.me.spells.myTransform);
                melee.comp.myTransform.position += melee.areaOfEffect * 0.5f * Br.myTransform.forward;
                melee.InitializeMe(Br);
                break;
            case 1: //projectile
                S_Bullet projectile = Instantiate(Ga.me.spells.projectileEnemy, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform) as S_Bullet;
                projectile.comp.myMesh.localPosition = spawnPoint.position.y * Vector3.up;
                projectile.ricochet = ricochet;
                projectile.pierce = pierce;
                projectile.bounce = bounce;
                projectile.InitializeMe(Br);
                break;
        }
    }

    public override void FromAnimEv_Ultimate(int num = 0)
    {
        base.FromAnimEv_Ultimate(num);
        switch (num)
        {
            case 0:
                Spell lob = Instantiate(Ga.me.spells.lobCarrierFireball,
                    spawnPoint.position,
                    Quaternion.identity, Ga.me.spells.myTransform);
                lob.InitializeMe(Br);
                break;
        }
    }
}