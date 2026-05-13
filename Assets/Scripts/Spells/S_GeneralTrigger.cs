using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class S_GeneralTrigger : Spell
{
    [Title("Triggers")]
    [SerializeField] bool onEnter = true;
    [SerializeField] bool onExit;

    //Only type matters. All instances of same type are treated the same. E.g., any 'S_Bullet' in array detects all variations. If 'MainSpell' is in array that detects all.
    bool IsNullify() => hitEffects.Contains(HitEffect.Nullify);
    [SerializeField, ShowIf(nameof(IsNullify))] SpellControl[] spellsToAffect;
    
    
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

        if (hitEffects.Contains(HitEffect.Nullify) &&
            other.TryGetComponent(out SpellControl spellControl) && 
            Utils.CanTargetFaction(myFaction, spellControl.spell.myFaction, myFactionTarget))
        {
            for (int i = 0; i < spellsToAffect.Length; i++)
            {
                if (spellControl.spell.GetType() != spellsToAffect[i].spell.GetType()) continue;
                spellControl.spell.MyPhase = Phase.EndStart;
                break;
            }
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
