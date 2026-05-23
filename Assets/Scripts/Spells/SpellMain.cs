using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class SpellMain : MonoBehaviour
{
    public bool debug;
    public Brain OwnersBrain
    {
        get => _ownersBrain;
        set
        {
            if (_ownersBrain != null)
            {
                Physics.IgnoreCollision(mySphereCollider, OwnersBrain.myCollider, false);
                Physics.IgnoreCollision(myCapsuleCollider, OwnersBrain.myCollider, false);
            }
            
            _ownersBrain = value;
            Physics.IgnoreCollision(mySphereCollider, OwnersBrain.myCollider, false);
            Physics.IgnoreCollision(myCapsuleCollider, OwnersBrain.myCollider, false);
            mySphereCollider.enabled = false;
            myCapsuleCollider.enabled = false;
            switch (colliderType)
            {
                case ColliderType.Sphere:
                    Physics.IgnoreCollision(mySphereCollider, OwnersBrain.myCollider);
                    mySphereCollider.enabled = true;
                    break;
                case ColliderType.Capsule:
                    Physics.IgnoreCollision(myCapsuleCollider, OwnersBrain.myCollider);
                    myCapsuleCollider.enabled = true;
                    break;
            }

        }
    }
    [ReadOnly, ShowInInspector] Brain _ownersBrain;
    public Transform myTransform;
    public Rigidbody myRigid;
    enum ColliderType { Sphere, Capsule, None }
    [SerializeField] ColliderType colliderType;

    public SphereCollider mySphereCollider;
    public CapsuleCollider myCapsuleCollider;
    public Transform myMesh;
    public SpriteRenderer warningRend;
    public Transform visualization;
    [ReadOnly] public Dictionary<Element, float> damage = new Dictionary<Element, float>();
    public Spell spell; 
    public SpellTransporter transporter;
    [SerializeField, TextArea, HideLabel] string description;

    #region EVENTS, CALLBACKS
    
    public System.Action onEnd;
    public System.Action<ITakeDamage> onHitTarget;
    System.Action _onAfterEffect;
    void CallEv_OnEnd()
    {
        _onAfterEffect?.Invoke();
        Destroy(gameObject);
    }
    #endregion

    /// <summary>
    /// Damage is from inspector, no after effect
    /// </summary>
    public void InitializeMe(Brain brain)
    {
        OwnersBrain = brain;
        onEnd += CallEv_OnEnd;
        transporter.InitializeMe(this);
    }
    /// <summary>
    /// Damage is from code, no after effect
    /// </summary>
    public void InitializeMe(Brain brain, Dictionary<Element, float> dam)
    {
        if (dam == null) dam = new Dictionary<Element, float>();
        damage = dam;
        InitializeMe(brain);
    }
    /// <summary>
    /// Damage is from inspector, with after effect
    /// </summary>
    public void InitializeMe(Brain brain, System.Action onAfterEffect)
    {
        _onAfterEffect = onAfterEffect;
        InitializeMe(brain);        
    }
    /// <summary>
    /// Damage is from code, with after effect
    /// </summary>
    public void InitializeMe(Brain brain, Dictionary<Element, float> dam, System.Action onAfterEffect)
    {
        if (dam == null) dam = new Dictionary<Element, float>();
        damage = dam;
        _onAfterEffect = onAfterEffect;
        InitializeMe(brain); 
    }

    void OnTriggerEnter(Collider other) => spell.CallEv_OnTriggerEnter(other);
    void OnTriggerExit(Collider other) => spell.CallEv_OnTriggerExit(other);
    void OnCollisionEnter(Collision collision) => spell.CallEv_OnCollisionEnter(collision);

}
