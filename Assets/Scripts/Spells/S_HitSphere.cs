using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// area effect, instantaneous
/// </summary>
public class S_HitSphere : Spell
{
    bool _oneHIt;
    
    protected override void Update()
    {
        base.Update();
        if (!_oneHIt && !timerStartDelay.completed) return;
        _oneHIt = true;
        Hit();
    }

    void Hit()
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
        
        spellParticles.InitializeMe(areaOfEffect);
        AfterEffect();

    }

}

