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
        if (!CheckColliderType(other.transform.position)) return;
        
        if (hitEffects.Contains(HitEffect.Damage) &&
            injectHealthData.damage.Count > 0 && 
            other.TryGetComponent(out ITakeDamage takeDamage) && 
            Utils.CanTargetFaction(main.OwnersBrain.Faction, takeDamage.Br.Faction, myFactionTarget))
        {
            takeDamage.TakeDamage(injectHealthData);
        }

        if (hitEffects.Contains(HitEffect.Nullify) &&
            other.TryGetComponent(out SpellMain mainToNullify) && 
            Utils.CanTargetFaction(main.OwnersBrain.Faction, mainToNullify.OwnersBrain.Faction, myFactionTarget))
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
            Utils.CanTargetFaction(main.OwnersBrain.Faction, mainToReflect.OwnersBrain.Faction, myFactionTarget))
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
        if (!CheckColliderType(other.transform.position)) return;
        if (hitEffects.Contains(HitEffect.StatChange) && collidersDetected.Contains(other))
        {
            //revert change
            collidersDetected.Remove(other);
        }

    }

    bool CheckColliderType(Vector3 pos)
    {
        float posZ =  main.myTransform.InverseTransformPoint(pos).z;
        switch (colliderPart)
        {
            case ColliderPart.FrontHalf:
                if (posZ < 0) return false;
                break;
            case ColliderPart.BackHalf:
                if (posZ > 0) return  false;
                break;
        }
        return true;
    }
}
