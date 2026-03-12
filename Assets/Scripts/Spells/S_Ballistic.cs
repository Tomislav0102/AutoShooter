using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class S_Ballistic : Spell
{
    [Title("Spell Name")]
    public override void InitializeMe(SpellData dat, System.Action<Transform> hitAction)
    {
        base.InitializeMe(dat, hitAction);
        myCollider.enabled = true;
        myRigid.linearVelocity = dat.speed * myTransform.forward;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(myData.damage);
          //  onHit?.Invoke(takeDamage.transform);
        }
        if (oneHitSwitch) OnEnd();
    }
}
