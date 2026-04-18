using System;
using UnityEngine;

/// <summary>
/// area effect, instantaneous
/// </summary>
public class S_HitSphere : Spell
{
    public override void InitializeMe(Brain brain, float dam)
    {
        base.InitializeMe(brain, dam);
        comp.mySphereCollider.enabled = true;
    }
    // public override void InitializeMe(Brain brain, float dam)
    // {
    //     base.InitializeMe(brain, dam);
    //     Collider[] colliders = Physics.OverlapSphere(comp.myTransform.position, areaOfEffect * 0.5f, Utils.LayHostiles(brain.faction));
    //     
    //     foreach (Collider item in colliders)
    //     {
    //         if (injectHealthData.knockBack > 0 && item.TryGetComponent(out Brain br))
    //         {
    //             if (br.loco != null)
    //             {
    //                 Vector3 dir = Utils.Direction(comp.myTransform.position, item.transform.position);
    //                 br.loco.KnockBack(dir, injectHealthData.knockBack);
    //             }
    //         }
    //         if (injectHealthData.damage > 0 && item.TryGetComponent(out ITakeDamage takeDamage))
    //         {
    //             takeDamage.TakeDamage(injectHealthData);
    //         }
    //     }
    //
    //     if (comp.myMesh != null)
    //     {
    //         comp.myMesh.localScale = areaOfEffect * Vector3.one;
    //         comp.myMesh.GetComponentInChildren<ParticleSystem>().Play();
    //     }
    //     AfterEffect();
    //    // OnEnd();
    // }

    void OnTriggerEnter(Collider other)
    {
        if (injectHealthData.knockBack > 0 && other.TryGetComponent(out Brain br))
        {
            if (br.loco != null)
            {
                Vector3 dir = Utils.Direction(comp.myTransform.position, br.myTransform.position);
                br.loco.KnockBack(dir, injectHealthData.knockBack);
            }
        }
        if (injectHealthData.damage > 0 && other.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(injectHealthData);
        }

        if (comp.myMesh != null)
        {
            comp.myMesh.localScale = areaOfEffect * Vector3.one;
            comp.myMesh.GetComponentInChildren<ParticleSystem>().Play();
        }

        AfterEffect();
    }
}
