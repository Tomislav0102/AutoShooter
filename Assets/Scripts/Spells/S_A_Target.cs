using System.Collections.Generic;
using UnityEngine;

public class S_A_Target : S_A
{
    Brain _anchorBrain;

    public override void InitializeMe(SpellMain mainSpell)
    {
        base.InitializeMe(mainSpell);
        if (main.transporter.myTarget == null || main.transporter.myTarget.GetComponent<Brain>() == null) MyPhase = Phase.EndStart;
        _anchorBrain = main.transporter.myTarget.GetComponent<Brain>();
    }

    protected override void Hit()
    {
        HitMethod(_anchorBrain.myCollider, out _);
        main.onHitTarget?.Invoke(_anchorBrain);
    }
}
