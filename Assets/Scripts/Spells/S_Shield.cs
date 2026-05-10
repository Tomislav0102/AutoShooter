using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class S_Shield : Spell
{
    [Title("Shield")]
    [InfoBox("Only type matters. All instances of same type are treated the same. E.g., any 'S_Bullet' in array detects all variations. If 'MainSpell' is in array that detects all.")]
    [SerializeField] SpellControl[] spellsToAffect;


    public override void InitializeMe(SpellControl mainSpell)
    {
        base.InitializeMe(mainSpell);
        spellParticles.InitializeMe(areaOfEffect);
    }

    protected override void CallEv_OnTriggerEnter(Collider other)
    {
        base.CallEv_OnTriggerEnter(other);
        if (other.TryGetComponent(out SpellControl spellControl) && 
            Utils.CanTargetFaction(myFaction, spellControl.spell.myFaction, myFactionTarget))
        {
            for (int i = 0; i < spellsToAffect.Length; i++)
            {
                if (spellControl.spell.GetType() != spellsToAffect[i].spell.GetType()) continue;
                spellControl.spell.MyPhase = Phase.EndStart;
                return;
            }
        }
    }
}
