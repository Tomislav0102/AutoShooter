using System;
using UnityEngine;

public class P_Knight : PlayerCombat
{
    [SerializeField] float dashPower = 4f;
    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        Spell melee = Instantiate(Ga.me.spells.meleeHard,
            Br.myTransform.position,
            Quaternion.identity, Ga.me.spells.myTransform).GetComponent<Spell>();
        melee.myTransform.position += melee.radius * Br.myTransform.forward;
        melee.InitializeMe(Br);
    }
    
    protected override void CallEv_OnSpecialActivated()
    {
        base.CallEv_OnSpecialActivated();
        Br.loco.Dash(Br.myTransform.forward, dashPower);
    }
}
