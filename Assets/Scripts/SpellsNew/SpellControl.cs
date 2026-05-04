using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpellControl : MonoBehaviour
{
    public Transform myTransform;
    public Rigidbody myRigid;
    public SphereCollider mySphereCollider;
    public CapsuleCollider myCapsuleCollider;
    public SphereCollider mySolidSphereCollider;
    public Transform myMesh;
    public SpriteRenderer warningRend;
    public Transform visualization;
    [ReadOnly] public Brain ownersBrain;
    [ReadOnly] public Dictionary<Element, float> damage;
    public Spell spell; //initialized in transporter
    public SpellTransporter transporter; //every spell has one
    [ReadOnly] public bool isActive;

    public System.Action<Collider> onTrigEnter;
    public System.Action<Collider> onTrigExit;
    public System.Action<Collision> onCollisionEnter;

    public void InitializeMe(Brain brain, Dictionary<Element, float> dam = null)
    {
        ownersBrain = brain;
        damage = dam;
        transporter.InitializeMe(this);
    }
    
    void OnTriggerEnter(Collider other)
    {
        onTrigEnter?.Invoke(other);
    }

    void OnTriggerExit(Collider other)
    {
        onTrigExit?.Invoke(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        onCollisionEnter?.Invoke(collision);
    }
}
