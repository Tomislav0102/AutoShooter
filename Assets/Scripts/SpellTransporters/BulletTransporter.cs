using System;
using UnityEngine;

public class BulletTransporter : SpellTransporter
{
    [SerializeField] float speed;
    public int ricochet;
    public int pierce;
    public int bounce;

    public override void InitializeMe(SpellControl spellControl, Action onAfterEffect = null)
    {
        base.InitializeMe(spellControl, onAfterEffect);
        main.spell.InitializeMe(main);
        main.myRigid.isKinematic = false;
        SetSpeed(speed);
    }

    public void BounceMethod(Vector3 normal)
    {
        bounce--;
        Vector3 dir = Vector3.Reflect(main.myTransform.forward, normal);
        main.myTransform.forward = dir.normalized;
        SetSpeed(speed);
    }

    public void RicochetMethod(Vector3 direction)
    {
        ricochet--;
        main.myTransform.forward = direction.normalized;
        SetSpeed(speed);
    }
}
