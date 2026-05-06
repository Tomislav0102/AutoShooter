using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpellControl : MonoBehaviour
{
    public Transform myTransform;
    public Rigidbody myRigid;
    public SphereCollider mySphereCollider;
    public CapsuleCollider myCapsuleCollider;
    public Transform myMesh;
    public SpriteRenderer warningRend;
    public Transform visualization;
    [ReadOnly] public Brain ownersBrain;
    [ReadOnly] public Dictionary<Element, float> damage;
    public Spell spell; //initialized in transporter
    public SpellTransporter transporter; //every spell has one

    public System.Action<Collider> onTrigEnter;
    public System.Action<Collider> onTrigExit;
    public System.Action<Collision> onCollisionEnter;
    public System.Action onEnd;
    System.Action _onAfterEffect;

    public void InitializeMe(Brain brain, Dictionary<Element, float> dam = null, System.Action onAfterEffect = null)
    {
        ownersBrain = brain;
        damage = dam;
        _onAfterEffect = onAfterEffect;
        onEnd += CallEv_OnEnd;
        transporter.InitializeMe(this);
    }

    void CallEv_OnEnd()
    {
        _onAfterEffect?.Invoke();
        Destroy(gameObject);
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
