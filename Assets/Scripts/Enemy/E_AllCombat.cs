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
                    { Element.Physical, Br.myChar.GetStat(Stats.MeleeDamage) },
                };
                Spell melee = Instantiate(meleeWeapon,
                    Br.myTransform.position,
                    Quaternion.identity, Ga.me.spells.myTransform);
                melee.comp.myTransform.position += melee.areaOfEffect * 0.5f * Br.myTransform.forward;
                melee.InitializeMe(Br, damMelee);
                break;
            case 1: //projectile
                damRanged = new Dictionary<Element, float>()
                {
                    { Element.Fire, Br.myChar.GetStat(Stats.RangedDamage) },
                };
                Spell projectile = Instantiate(rangedWeapon, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                projectile.comp.myMesh.localPosition = spawnPoint.position.y * Vector3.up;
                projectile.InitializeMe(Br, damRanged);
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
                lob.InitializeMe(Br, new Dictionary<Element, float>());
                break;
        }
    }
}