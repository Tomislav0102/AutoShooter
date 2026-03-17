using System;
using UnityEngine;

public class S_HitSphere : Spell
{
    public override void InitializeMe(Brain brain)
    {
        base.InitializeMe(brain);
        HitArea();
    }

    void HitArea()
    {
        Collider[] colliders = Physics.OverlapSphere(myTransform.position, radius,Utils.LayHostiles(faction));
        
        foreach (Collider item in colliders)
        {
            if (item.TryGetComponent(out ITakeDamage takeDamage))
            {
                takeDamage.TakeDamage(dam);
            }
        }
        OnEnd();
    }

}
