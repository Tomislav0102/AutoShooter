using System.Collections.Generic;
using UnityEngine;

public class S_A_Target : S_A
{
    Brain _anchorBrain;

    public override void InitializeMe(SpellMain mainSpell)
    {
        base.InitializeMe(mainSpell);
        if (main.transporter.target == null || main.transporter.target.GetComponent<Brain>() == null) MyPhase = Phase.EndStart;
        _anchorBrain = main.transporter.target.GetComponent<Brain>();
    }

    protected override void Hit()
    {
        base.Hit();
        HitGeneric(_anchorBrain, out _);
        main.onHitTarget?.Invoke(_anchorBrain);
    }
}
