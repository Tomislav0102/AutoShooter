using System;
using UnityEngine;

public class PlKnight : PlProfession
{
    [SerializeField] float attackRange;
    AttackOverlapSphere _sphere;
    public float dashForce;
    void Start()
    {
        _sphere = new AttackOverlapSphere(transform, attackRange, gm.layEnemies);
    }

    public override void AttackAnimEvent()
    {
        base.AttackAnimEvent();
        _sphere.Attack(damage);
    }

    protected override void CallEv_OnSpecialActivated()
    {
        base.CallEv_OnSpecialActivated();
        control.myRigid.AddRelativeForce(dashForce * Vector3.forward, ForceMode.VelocityChange);
    }
}
