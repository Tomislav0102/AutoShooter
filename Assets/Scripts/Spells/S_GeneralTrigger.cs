using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class S_GeneralTrigger : Spell
{
    [Title("Triggers")]
    [SerializeField] bool onEnter = true;
    [SerializeField] bool onExit;

    enum ColliderPart
    {
        Whole,
        FrontHalf,
        BackHalf,
    }
    [SerializeField] ColliderPart colliderPart;
    float MyZ(Vector3 pos) => main.myTransform.InverseTransformPoint(pos).z;
        
    //Only type matters. All instances of same type are treated the same. E.g., any 'S_Bullet' in array detects all variations. If 'MainSpell' is in array that detects all.
    bool IsNullify() => hitEffects.Contains(HitEffect.Nullify) || hitEffects.Contains(HitEffect.Reflect);
    [SerializeField, ShowIf(nameof(IsNullify))] SpellMain[] spellsToAffect;
    
    
    public override void InitializeMe(SpellMain mainSpell)
    {
        base.InitializeMe(mainSpell);
        spellParticles.InitializeMe(mainSpell);
    }

    protected override void CallEv_OnTriggerEnter(Collider other)
    {
        base.CallEv_OnTriggerEnter(other);
        if (!onEnter) return;
        switch (colliderPart)
        {
            case ColliderPart.FrontHalf:
                if (MyZ(other.transform.position) < 0) return;
                break;
            case ColliderPart.BackHalf:
                if (MyZ(other.transform.position) > 0) return;
                break;
        }
        
        if (hitEffects.Contains(HitEffect.Damage) &&
            injectHealthData.damage.Count > 0 && 
            other.TryGetComponent(out ITakeDamage takeDamage) && 
            Utils.CanTargetFaction(main.OwnersBrain.faction, takeDamage.Br.faction, myFactionTarget))
        {
            takeDamage.TakeDamage(injectHealthData);
        }

        if (hitEffects.Contains(HitEffect.Nullify) &&
            other.TryGetComponent(out SpellMain mainToNullify) && 
            Utils.CanTargetFaction(main.OwnersBrain.faction, mainToNullify.OwnersBrain.faction, myFactionTarget))
        {
            for (int i = 0; i < spellsToAffect.Length; i++)
            {
                if (mainToNullify.spell.GetType() != spellsToAffect[i].spell.GetType()) continue;
                mainToNullify.spell.MyPhase = Phase.EndStart;
                break;
            }
        }
        if (hitEffects.Contains(HitEffect.Reflect) &&
            other.TryGetComponent(out SpellMain mainToReflect) && 
            Utils.CanTargetFaction(main.OwnersBrain.faction, mainToReflect.OwnersBrain.faction, myFactionTarget))
        {
            for (int i = 0; i < spellsToAffect.Length; i++)
            {
                if (mainToReflect.spell.GetType() != spellsToAffect[i].spell.GetType()) continue;
                mainToReflect.transporter.ReflectProjectile(main.OwnersBrain);
                break;
            }
        }

        if (hitEffects.Contains(HitEffect.StatChange) && !collidersDetected.Contains(other))
        {
            collidersDetected.Add(other);
            //change stats
        }
        
        main.spell.MyPhase = Phase.EndStart;
    }

    
    protected override void CallEv_OnTriggerExit(Collider other)
    {
        base.CallEv_OnTriggerExit(other);
        if (!onExit) return;
        switch (colliderPart)
        {
            case ColliderPart.FrontHalf:
                if (MyZ(other.transform.position) < 0) return;
                break;
            case ColliderPart.BackHalf:
                if (MyZ(other.transform.position) > 0) return;
                break;
        }

        if (hitEffects.Contains(HitEffect.StatChange) && collidersDetected.Contains(other))
        {
            //revert change
            collidersDetected.Remove(other);
        }

    }
}
