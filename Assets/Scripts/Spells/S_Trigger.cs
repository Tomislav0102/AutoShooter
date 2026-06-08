using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class S_Trigger : Spell
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
        
    //Only type matters. All instances of same type are treated the same. E.g., any 'S_Bullet' in array detects all variations. If 'Spell' is in array that detects all.
    bool AffectsSpells() => hitEffects.Contains(HitEffect.Nullify) || hitEffects.Contains(HitEffect.Reflect);
    [SerializeField, ShowIf(nameof(AffectsSpells))] SpellMain[] spellsToAffect;
    
    
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
        if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;
        SpellMain[] sps = AffectsSpells() ? spellsToAffect : null;
        HitMethod(other, out Brain targetBrain, sps);
        main.onHitTarget?.Invoke(targetBrain);
        main.spell.MyPhase = Phase.EndStart;
    }

    
    public override void OnTriggerExitCallBack(Collider other)
    {
        base.OnTriggerExitCallBack(other);
        if (!onExit) return;
        if (!CheckColliderType(other.transform.position)) return;
        if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;
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
