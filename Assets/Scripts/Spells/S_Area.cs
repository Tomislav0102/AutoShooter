using System;
using UnityEngine;

public class S_Area : Spell
{
    protected override void InitializeMe(Action<Transform> hitAction = null)
    {
        base.InitializeMe(hitAction);
        HitArea();
    }

    void HitArea()
    {
        Collider[] colliders = Physics.OverlapSphere(this.myTransform.position, 
                                                    myTransform.localScale.x * 0.5f, 
                                                    myData.layTarget);
        foreach (Collider item in colliders)
        {
            if (item.TryGetComponent(out ITakeDamage takeDamage))
            {
                onHit?.Invoke(takeDamage.Br.loco.myTransform);
                takeDamage.TakeDamage(myData.damage);
            }
        }
        OnEnd();
    }
}
