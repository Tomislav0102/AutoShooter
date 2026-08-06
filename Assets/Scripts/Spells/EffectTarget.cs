using System.Collections.Generic;
using UnityEngine;


public class EffectTarget : SpellEffect
{
    public override SpellMain Spell
    {
        get => base.Spell;
        set
        {
            base.Spell = value;
            value.mySphereCollider.enabled = false;
            value.myCapsuleCollider.enabled = false;
            if (value.transporter.target == null || value.transporter.target.GetComponent<Brain>() == null) value.MyPhase = SpellMain.Phase.EndStart;
            _anchorBrain = value.transporter.target.GetComponent<Brain>();

        }
    }

    Brain _anchorBrain;


    public void Hit()
    {
        Spell.HitGeneric(_anchorBrain, out _);
        Spell.onHitTarget?.Invoke(_anchorBrain);
    }
}
