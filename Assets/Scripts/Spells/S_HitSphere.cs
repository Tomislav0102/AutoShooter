using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// area effect, instantaneous
/// </summary>
public class S_HitSphere : Spell
{
    public override void InitializeMe(Brain brain, Dictionary<Element, float> damage)
    {
        base.InitializeMe(brain, damage);
        if (brain.myTransform == Ga.me.playerTransform)
        {
            print(gameObject.layer);
            print(Ga.me.gameData.TargetLayer(myFaction, targetFaction).value);
        }
        Collider[] colliders = Physics.OverlapSphere(comp.myTransform.position, 
            areaOfEffect * 0.5f, 
            Ga.me.gameData.TargetLayer(myFaction, targetFaction));
        foreach (Collider item in colliders)
        {
            if (injectHealthData.knockBack > 0 && item.TryGetComponent(out Brain br))
            {
                if (br.loco != null)
                {
                    Vector3 dir = Utils.Direction(comp.myTransform.position, br.myTransform.position);
                    br.loco.KnockBack(dir, injectHealthData.knockBack);
                }
            }
            if (injectHealthData.damage.Count > 0 && item.TryGetComponent(out ITakeDamage takeDamage))
            {
                takeDamage.TakeDamage(injectHealthData);
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
