using UnityEngine;

public class S_Lob : Spell
{
    public override void InitializeMe(Brain brain)
    {
        base.InitializeMe(brain);
        myTransform.rotation *= Quaternion.Euler(-45f, Random.Range(0f, 360f), 0);
        myRigid.isKinematic = false;
        myRigid.useGravity = true;
        myRigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        myRigid.AddForce(speed * Random.Range(0.6f, 1.4f) * myTransform.forward, ForceMode.VelocityChange);
    }

    protected override void Update()
    {
        base.Update();
        if (myTransform.position.y <= 0.2f) //activates on ground level only, no contacts/triggers/collisions
        {
            if (afterEffect != null)
            {
                Spell spell = Instantiate(afterEffect, myTransform.position, Quaternion.identity, Ga.me.spells.myTransform).GetComponent<Spell>();
                spell.InitializeMe(ownersBrain);
            }
            OnEnd();
        }
    }
}
