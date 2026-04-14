using UnityEngine;

public class S_Lob : Spell
{
    public override void InitializeMe(Brain brain)
    {
        base.InitializeMe(brain);
        comp.myTransform.rotation *= Quaternion.Euler(-45f, Random.Range(0f, 360f), 0);
        comp.myRigid.isKinematic = false;
        comp.myRigid.useGravity = true;
        comp.myRigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        comp.myRigid.AddForce(speed * Random.Range(0.6f, 1.4f) * comp.myTransform.forward, ForceMode.VelocityChange);
    }

    protected override void Update()
    {
        base.Update();
        if (comp.myTransform.position.y <= 0.2f) //activates on ground level only, no contacts/triggers/collisions
        {
            AfterEffect();
            OnEnd();
        }
    }
}
