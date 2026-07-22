using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class SpellMain : MonoBehaviour
{
    [BoxGroup] public bool debug;
    [ReadOnly] public bool mainActive = true;
    public bool isInterrupt;

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
    [ReadOnly] public PassDataContainer pd;
    public Spell spell; 
    public SpellTransporter transporter;
    public SpellVisual visual;
    [SerializeField, TextArea, HideLabel] string description;


    #region INITIALIZATION
    /// <summary>
    /// Damage is from inspector, no after spell
    /// </summary>
    public void InitializeMe(Brain brain)
    {
        if (brain == null) //it's a hack, but it works
        {
            Destroy(gameObject);
            return;
        }
        OwnersBrain = brain;
        onEnd += CallEv_OnEnd;
        myRigid.isKinematic = true;
        if (isInterrupt) gameObject.layer = LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt);
        pd.myBrain = OwnersBrain;
        transporter.InitializeMe(this);
        spell.InitializeMe(this);
        visual.InitializeMe(this);
    }
    /// <summary>
    /// Damage is from code, no after spell
    /// </summary>
    public void InitializeMe(Brain brain, PassDataContainer passData)
    {
        pd = passData;
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
    public void InitializeMe(Brain brain, PassDataContainer passData, System.Action onAfterSpell)
    {
        pd = passData;
        _onAfterSpell = onAfterSpell;
        InitializeMe(brain); 
    }
    #endregion

    #region EVENTS, CALLBACKS
    public System.Action onEnd;
    public System.Action<Brain> onHitTarget;
    System.Action _onAfterSpell;
    void CallEv_OnEnd()
    {
        _onAfterSpell?.Invoke();
        mainActive = false;
        Destroy(gameObject);
    }
    #endregion

    #region TRIGGERS/COLLSIONS
    
    bool UseTriggers() => mainActive && Random.value < spell.hitChance;
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
    #endregion

}
