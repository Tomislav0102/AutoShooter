using System;
using UnityEngine;

/// <summary>
/// area effect, instantaneous
/// </summary>
public class S_HitSphere : Spell
{
    public override void InitializeMe(Brain brain)
    {
        base.InitializeMe(brain);
        
        Collider[] colliders = Physics.OverlapSphere(myTransform.position, radius);
        
        foreach (Collider item in colliders)
        {
            if (knockBack > 0 && item.TryGetComponent(out Brain br) && factionsToTarget.Contains(br.faction))
            {
                if (br.loco != null)
                {
                    Vector3 dir = Utils.Direction(myTransform.position, item.transform.position);
                    br.loco.KnockBack(dir, knockBack);
                }
            }
            if (damage > 0 && item.TryGetComponent(out ITakeDamage takeDamage) && factionsToTarget.Contains(takeDamage.Br.faction))
            {
                takeDamage.TakeDamage(dam);
            }
        }
        AfterEffect();
        OnEnd();
    }
}
