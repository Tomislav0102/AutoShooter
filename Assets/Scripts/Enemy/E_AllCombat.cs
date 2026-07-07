using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class E_AllCombat : EnemyCombat
{
    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        switch (num)
        {
            case 0: //melee
                damMelee = new Dictionary<Element, float>()
                {
                    { Element.Physical, Br.character.GetStat(Stats.MeleeDamage) },
                };
                SpellMain melee = Instantiate(meleeWeapon,
                    Br.myTransform.position,
                    Br.myTransform.rotation, Ga.me.spells.myTransform);
                injectHealth = new InjectHealth(damMelee, true);
                melee.InitializeMe(Br, injectHealth);
                break;
            case 1: //bullet
                damRanged = new Dictionary<Element, float>()
                {
                    { Element.Fire, Br.character.GetStat(Stats.RangedDamage) },
                };
                Vector3 zeroSpawnPoint = new Vector3(spawnPoint.position.x, 0f, spawnPoint.position.z);
                SpellMain bullet = Instantiate(rangedWeapon, zeroSpawnPoint, Br.myTransform.rotation, Ga.me.spells.myTransform);
                bullet.visual.SetSpawnHeight(spawnPoint.position.y);
                injectHealth = new InjectHealth(damRanged, true);
                bullet.InitializeMe(Br, injectHealth);
                break;
            case 2: //lightning strike
                break;
        }
    }

    public override void FromAnimEv_Ultimate(int num = 0)
    {
        base.FromAnimEv_Ultimate(num);
        switch (num)
        {
            case 0:
                damRanged = new Dictionary<Element, float>()
                {
                    { Element.Fire, Br.character.GetStat(Stats.RangedDamage) },
                };

                SpellMain lob = Instantiate(rangedWeapon, spawnPoint.position, Quaternion.identity, Ga.me.spells.myTransform);
                injectHealth = new InjectHealth(damRanged);
                lob.InitializeMe(Br, new InjectHealth(new Dictionary<Element, float>()), () =>
                {
                    SpellMain explosion = Instantiate(Ga.me.spells.explosionFire, lob.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    explosion.InitializeMe(Br, injectHealth);
                });
                break;
        }
        
    }
}