using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class HomingTransporter : SpellTransporter
{
    [ReadOnly] public Transform homingTarget;
    [SerializeField] float speed;
    
    public override void InitializeMe(SpellControl spellControl, System.Action onAfterEffect = null)
    {
        base.InitializeMe(spellControl, onAfterEffect);
        main.spell.InitializeMe(main);
        main.myRigid.isKinematic = false;
    }

    void Update()
    {
        if (homingTarget == null) return;
        Vector3 dir = Utils.Direction(main.myTransform.position, homingTarget.position);
        main.myTransform.rotation = Quaternion.Slerp(main.myTransform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);
    }
    
    void FixedUpdate()
    {
        SetSpeed(speed);
    }

}
