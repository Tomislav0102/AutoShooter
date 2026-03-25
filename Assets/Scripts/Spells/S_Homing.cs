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
        myCollider.enabled = true;
        myRigid.isKinematic = false;
    }

    // protected override void Update()
    // {
    //     base.Update();
    //     if (homingTarget == null) return;
    //     Vector3 dir = Utils.Direction(myTransform.position, homingTarget.position);
    //     myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);
    // }

    void FixedUpdate()
    {
        SetSpeed();
    }

    void OnTriggerEnter(Collider other)
    {
        if (damage > 0 && other.TryGetComponent(out ITakeDamage takeDamage) && takeDamage.Br.faction != faction)
        {
            takeDamage.TakeDamage(dam);
        }
        AfterEffect();

        OnEnd();
    }
}
