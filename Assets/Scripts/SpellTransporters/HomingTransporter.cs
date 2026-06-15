using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class HomingTransporter : SpellTransporter
{
    [SerializeField] float speed;
    
    public override void InitializeMe(SpellMain spellMain)
    {
        base.InitializeMe(spellMain);
        main.myRigid.isKinematic = false;
    }

    void Update()
    {
        if (!main.IsActive ) return;
        if (myTarget == null) return;
        Vector3 dir = Utils.Direction(main.myTransform.position, myTarget.position);
        main.myTransform.rotation = Quaternion.Slerp(main.myTransform.rotation, Quaternion.LookRotation(dir), speed * Time.deltaTime);
    }
    
    void FixedUpdate()
    {
        SetSpeed(speed);
    }

}
