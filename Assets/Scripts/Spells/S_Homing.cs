using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class S_Homing : Spell
{
    [Title("Homing")]
    public Transform homingTarget;
    
    public override void InitializeMe(Brain brain, Dictionary<Element, float> damage, float delay = 0f)
    {
        base.InitializeMe(brain, damage, delay);
        comp.mySphereCollider.enabled = true;
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
        if (injectHealthData.damage.Count > 0 && 
            other.TryGetComponent(out ITakeDamage takeDamage) && 
            Utils.CanTargetFaction(myFaction, takeDamage.Br.faction, myFactionTarget))
        {
            takeDamage.TakeDamage(injectHealthData);
        }
        AfterEffect();
        OnEnd();
    }
}
