using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// area effect, instantaneous
/// </summary>
public class S_HitSphere : Spell
{
    public override void InitializeMe(Brain brain, Dictionary<Element, float> damage, float delay = 0f)
    {
        base.InitializeMe(brain, damage, delay);
    }
    
    void Hit(Brain brain, Dictionary<Element, float> damage)
    {
        Collider[] colliders = Physics.OverlapSphere(comp.myTransform.position, 
            areaOfEffect * 0.5f);
        foreach (Collider item in colliders)
        {
            if (item.TryGetComponent(out Brain collidersBrain))
            {
                if (!Utils.CanTargetFaction(myFaction, collidersBrain.faction, myFactionTarget)) continue;
                if (injectHealthData.knockBack > 0 && collidersBrain.loco != null)
                {
                    Vector3 dir = Utils.Direction(comp.myTransform.position, collidersBrain.myTransform.position);
                    collidersBrain.loco.KnockBack(dir, injectHealthData.knockBack);
                }
                if (injectHealthData.damage.Count > 0 && item.TryGetComponent(out ITakeDamage takeDamage))
                {
                    takeDamage.TakeDamage(injectHealthData);
                }
            }
        }
        
        if (comp.myMesh != null)
        {
            comp.myMesh.localScale = areaOfEffect * Vector3.one;
            comp.myMesh.GetComponentInChildren<ParticleSystem>().Play();
        }
        AfterEffect();

    }

}

