using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class P_Barb : PlayerCombat
{
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            damMelee = new Dictionary<Element, float>()
            {
                { Element.Physical, Br.myChar.GetStat(Stats.MeleeDamage) },
            };
            IsInitialized = true;
        }
    }

    [SerializeField] Transform weaponTr;
    float _weaponAngle;



    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);

        // Spell melee = Instantiate(Ga.me.spells.meleePlayer, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        // Vector3 pos = melee.comp.myTransform.position +
        //               melee.areaOfEffect * 0.5f * Vector3.ProjectOnPlane(weaponTr.forward, Vector3.up);
        // melee.comp.myTransform.position = pos;
        // melee.InitializeMe(Br, damMelee);

    }
}
