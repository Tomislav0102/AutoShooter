using System;
using UnityEngine;

public class S_HitSphere : Spell
{
    protected override void InitializeMe()
    {
        base.InitializeMe();
        HitArea();
    }

    void HitArea()
    {
        Collider[] colliders = Physics.OverlapSphere(this.myTransform.position, 
                                                    myTransform.localScale.x * 0.5f, 
                                                    Utils.LayHostiles(myData.faction));
        foreach (Collider item in colliders)
        {
            if (item.TryGetComponent(out ITakeDamage takeDamage))
            {
                takeDamage.TakeDamage(myData.damage);
            }
        }
        OnEnd();
    }

}
