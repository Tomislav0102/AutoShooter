using System;
using UnityEngine;

public class P_Knight : PlayerCombat
{

    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        Spell melee = Instantiate(Ga.me.spells.melee,
            Br.loco.myTransform.position,
            Quaternion.identity, Ga.me.spells.myTransform).GetComponent<Spell>();
        melee.myTransform.position += melee.attackRange * Br.loco.myTransform.forward;
        melee.InitializeMe(Br.faction);
    }
    
    protected override void CallEv_OnSpecialActivated()
    {
        base.CallEv_OnSpecialActivated();
        StartCoroutine(Br.loco.Dash());
    }
}
