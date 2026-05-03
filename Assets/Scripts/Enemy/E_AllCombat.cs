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
                SpellControl melee = Instantiate(meleeWeapon,
                    Br.myTransform.position,
                    Br.myTransform.rotation, Ga.me.spells.myTransform);
                melee.InitializeMe(Br, damMelee);
                break;
            // case 1: //bullet
            //     damRanged = new Dictionary<Element, float>()
            //     {
            //         { Element.Fire, Br.myChar.GetStat(Stats.RangedDamage) },
            //     };
            //     Spell bullet = Instantiate(rangedWeapon, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
            //     bullet.comp.myMesh.localPosition = spawnPoint.position.y * Vector3.up;
            //     bullet.InitializeMe(Br, damRanged);
            //     break;
            // case 2: //lightning strike
            //     damRanged = new Dictionary<Element, float>()
            //     {
            //         { Element.Electricity, Br.myChar.GetStat(Stats.MagicDamage) },
            //     };
            //     Spell strike = Instantiate(rangedWeapon, Br.combat.MyTarget.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
            //     strike.comp.myMesh.localPosition = spawnPoint.position.y * Vector3.up;
            //     strike.InitializeMe(Br, damRanged);
            //     break;
        }
    }

    public override void FromAnimEv_Ultimate(int num = 0)
    {
        base.FromAnimEv_Ultimate(num);
        // switch (num)
        // {
        //     case 0:
        //         Spell lob = Instantiate(Ga.me.spells.lobCarrierFireball,
        //             spawnPoint.position,
        //             Quaternion.identity, Ga.me.spells.myTransform);
        //         lob.InitializeMe(Br, new Dictionary<Element, float>());
        //         break;
        // }
    }
}