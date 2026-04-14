using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class S_Homing : Spell
{
    [Title("Homing")]
    public Transform homingTarget;
    
    public override void InitializeMe(Brain brain)
    {
        base.InitializeMe(brain);
        comp.myCollider.enabled = true;
        comp.myRigid.isKinematic = false;
    }

    protected override void Update()
    {
        base.Update();
        if (homingTarget == null) return;
        Vector3 dir = Utils.Direction(comp.myTransform.position, homingTarget.position);
        comp.myTransform.rotation = Quaternion.Slerp(comp.myTransform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);
    }

    void FixedUpdate()
    {
        SetSpeed();
    }

    void OnTriggerEnter(Collider other)
    {
        if (damageMod > 0 && other.TryGetComponent(out ITakeDamage takeDamage) && factionsToTarget.Contains(takeDamage.Br.faction))
        {
            takeDamage.TakeDamage(dam);
        }
        AfterEffect();
        OnEnd();
    }
}
