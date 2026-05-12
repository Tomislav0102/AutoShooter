using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class S_SimpleTrigger : Spell
{
    [Title("Triggers")]
    [SerializeField] bool onEnter = true;
    [SerializeField] bool onExit;

    public override void InitializeMe(SpellControl mainSpell)
    {
        base.InitializeMe(mainSpell);
        spellParticles.InitializeMe(areaOfEffect);
    }

    protected override void CallEv_OnTriggerEnter(Collider other)
    {
        base.CallEv_OnTriggerEnter(other);
        if (!onEnter) return;
        
        if (hitEffects.Contains(HitEffect.Damage) &&
            injectHealthData.damage.Count > 0 && 
            other.TryGetComponent(out ITakeDamage takeDamage) && 
            Utils.CanTargetFaction(myFaction, takeDamage.Br.faction, myFactionTarget))
        {
            takeDamage.TakeDamage(injectHealthData);
        }

        if (hitEffects.Contains(HitEffect.StatChange))
        {
            
        }
        
        main.spell.MyPhase = Phase.EndStart;
    }

    protected override void CallEv_OnTriggerExit(Collider other)
    {
        base.CallEv_OnTriggerExit(other);
        if (!onExit) return;
    }
}
