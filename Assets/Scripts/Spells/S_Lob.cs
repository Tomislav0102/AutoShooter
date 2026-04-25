using System.Collections.Generic;
using UnityEngine;

public class S_Lob : Spell
{
    Vector3 _rndRot;
    public override void InitializeMe(Brain brain, Dictionary<Element, float> damage, float delay)
    {
        base.InitializeMe(brain, damage);
        comp.myTransform.rotation *= Quaternion.Euler(-45f, Random.Range(0f, 360f), 0);
        comp.myRigid.isKinematic = false;
        comp.myRigid.useGravity = true;
        comp.myRigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        comp.myRigid.AddForce(speed * Random.Range(0.6f, 1.4f) * comp.myTransform.forward, ForceMode.VelocityChange);
        _rndRot = 500f * Random.insideUnitSphere;
    }

    protected override void Update()
    {
        base.Update();
        comp.myMesh.Rotate(Time.deltaTime * _rndRot);
        if (!Ga.me.LevelMan.InsideLevel(comp.myTransform.position)) 
        {
            AfterEffect();
            OnEnd();
        }
    }
}
