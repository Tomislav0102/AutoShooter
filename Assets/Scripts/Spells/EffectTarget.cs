using System.Collections.Generic;
using UnityEngine;


public class EffectTarget : MonoBehaviour, IIniSpell
{
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value;
            value.mySphereCollider.enabled = false;
            value.myCapsuleCollider.enabled = false;
            if (value.transporter.target == null || value.transporter.target.GetComponent<Brain>() == null) value.MyPhase = SpellMain.Phase.EndStart;
            _anchorBrain = value.transporter.target.GetComponent<Brain>();

        }
    }
    SpellMain _spell;
    
    Brain _anchorBrain;


    public void Hit()
    {
        Spell.HitGeneric(_anchorBrain, out _);
        Spell.onHitTarget?.Invoke(_anchorBrain);
    }
}
