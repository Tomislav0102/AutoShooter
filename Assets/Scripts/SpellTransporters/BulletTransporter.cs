using System;
using UnityEngine;

public class BulletTransporter : SpellTransporter
{
    public override SpellMain Spell
    {
        get => base.Spell;
        set
        {
            base.Spell = value;
            value.myRigid.isKinematic = false;
            SetSpeed(speed);

        }
    }
    public float speed;
    public int ricochet;
    public int pierce;
    public int bounce;

    public void BounceMethod(Vector3 normal)
    {
        bounce--;
        Vector3 dir = Vector3.Reflect(Spell.myTransform.forward, normal);
        Spell.myTransform.forward = dir.normalized;
        SetSpeed(speed);
    }

    public void RicochetMethod(Vector3 direction)
    {
        ricochet--;
        Spell.myTransform.forward = direction.normalized;
        SetSpeed(speed);
    }
}
