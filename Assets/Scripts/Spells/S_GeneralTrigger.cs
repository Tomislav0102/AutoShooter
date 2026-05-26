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

    public override void OnTriggerEnterCallBack(Collider other)
    {
        base.OnTriggerEnterCallBack(other);
        if (!onEnter) return;
        if (!CheckColliderType(other.transform.position)) return;
        
        if (other.TryGetComponent(out Brain targetBrain))
        {
            main.onHitTarget?.Invoke(targetBrain);
            if (hitEffects.Contains(HitEffect.Damage) &&
                injectHealthData.damage.Count > 0 && 
                Utils.CanTargetFaction(main.OwnersBrain.Faction, targetBrain.Faction, myFactionTarget))
            {
                targetBrain.health.TakeDamage(injectHealthData);
            }
            
            if (hitEffects.Contains(HitEffect.StatChange) && !collidersDetected.Contains(other))
            {
                collidersDetected.Add(other);
                //change stats
            }
        }
        
        if (other.TryGetComponent(out SpellMain targetSpell) && 
            Utils.CanTargetFaction(main.OwnersBrain.Faction, targetSpell.OwnersBrain.Faction, myFactionTarget))
        {
            for (int i = 0; i < spellsToAffect.Length; i++)
            {
                if (targetSpell.spell.GetType() != spellsToAffect[i].spell.GetType()) continue;
                main.onHitTarget?.Invoke(targetSpell.OwnersBrain); //might not work
                
                if (hitEffects.Contains(HitEffect.Nullify))  targetSpell.spell.MyPhase = Phase.EndStart;
                
                if (hitEffects.Contains(HitEffect.Reflect))
                {
                    Vector3 newDirection = Utils.Direction(main.myTransform.position, targetSpell.myTransform.position);
                    targetSpell.transporter.ReflectProjectile(main.OwnersBrain, newDirection);
                }
            }
        }
        
        main.spell.MyPhase = Phase.EndStart;
    }

    
    public override void OnTriggerExitCallBack(Collider other)
    {
        base.OnTriggerExitCallBack(other);
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
