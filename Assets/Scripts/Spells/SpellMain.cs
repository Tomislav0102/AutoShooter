using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class SpellMain : MonoBehaviour
{
    public bool debug;
    [ReadOnly] public bool IsActive = true;

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
    
    public ColliderType colliderType;

    public SphereCollider mySphereCollider;
    public CapsuleCollider myCapsuleCollider;
    public SpriteRenderer warningRend;
    [ReadOnly] public InjectHealth injectHealthPass;
    public Spell spell; 
    public SpellTransporter transporter;
    public SpellVisual visual;
    [SerializeField, TextArea, HideLabel] string description;

    #region EVENTS, CALLBACKS
    public System.Action onEnd;
    public System.Action<Brain> onHitTarget;
    System.Action _onAfterSpell;
    void CallEv_OnEnd()
    {
        _onAfterSpell?.Invoke();
        IsActive = false;
        Destroy(gameObject);
    }
    #endregion

    /// <summary>
    /// Damage is from inspector, no after spell
    /// </summary>
    public void InitializeMe(Brain brain)
    {
        OwnersBrain = brain;
        onEnd += CallEv_OnEnd;
        myRigid.isKinematic = true;
        transporter.InitializeMe(this);
        spell.InitializeMe(this);
        visual.InitializeMe(this);
    }
    /// <summary>
    /// Damage is from code, no after spell
    /// </summary>
    public void InitializeMe(Brain brain, InjectHealth inject)
    {
        if (inject == null) inject = new InjectHealth();
        injectHealthPass = inject;
        InitializeMe(brain); 
    }
    /// <summary>
    /// Damage is from inspector, with after spell
    /// </summary>
    public void InitializeMe(Brain brain, System.Action onAfterSpell)
    {
        _onAfterSpell = onAfterSpell;
        InitializeMe(brain);        
    }
    /// <summary>
    /// Damage is from code, with after spell
    /// </summary>
    public void InitializeMe(Brain brain, InjectHealth inject, System.Action onAfterSpell)
    {
        if (inject == null) inject = new InjectHealth();
        injectHealthPass = inject;
        _onAfterSpell = onAfterSpell;
        InitializeMe(brain); 
    }


    bool UseTriggers() => IsActive && Random.value < spell.hitChance;
    void OnTriggerEnter(Collider other)
    {
        if (UseTriggers()) spell.OnTriggerEnterCallBack(other);
    }
    void OnTriggerExit(Collider other)
    {
        if (UseTriggers()) spell.OnTriggerExitCallBack(other);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (UseTriggers()) spell.OnCollisionEnterCallBack(collision);
    }

}
