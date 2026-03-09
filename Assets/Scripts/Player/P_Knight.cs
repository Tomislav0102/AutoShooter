using System;
using UnityEngine;

public class P_Knight : PlayerCombat
{
    AttackOverlapSphere _sphere;
    void Start()
    {
        _sphere = new AttackOverlapSphere(transform, rangeMelee, gm.layEnemies);
    }

    public override void AE_Attack(int num = 0)
    {
        base.AE_Attack(num);
        _sphere.Attack(damage);
    }
    
    protected override void CallEv_OnSpecialActivated()
    {
        base.CallEv_OnSpecialActivated();
        StartCoroutine(br.loco.Dash());
    }
}
