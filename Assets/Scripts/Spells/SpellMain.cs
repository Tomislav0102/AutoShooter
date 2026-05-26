using System.Collections;
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
    public System.Action<Brain> onHitTarget;
    System.Action _onAfterSpell;
    void CallEv_OnEnd()
    {
        _onAfterSpell?.Invoke();
        StartCoroutine(DelayForParticles());

        IEnumerator DelayForParticles()
        {
            spell.IsActive = false;
            yield return new WaitForSeconds(5f);
            Destroy(gameObject);
        }
    }
    #endregion

    /// <summary>
    /// Damage is from inspector, no after spell
    /// </summary>
    public void InitializeMe(Brain brain)
    {
        OwnersBrain = brain;
        onEnd += CallEv_OnEnd;
        transporter.InitializeMe(this);
    }
    /// <summary>
    /// Damage is from code, no after spell
    /// </summary>
    public void InitializeMe(Brain brain, Dictionary<Element, float> dam)
    {
        if (dam == null) dam = new Dictionary<Element, float>();
        damage = dam;
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
    public void InitializeMe(Brain brain, Dictionary<Element, float> dam, System.Action onAfterSpell)
    {
        if (dam == null) dam = new Dictionary<Element, float>();
        damage = dam;
        _onAfterSpell = onAfterSpell;
        InitializeMe(brain); 
    }

    void OnTriggerEnter(Collider other)
    {
        if (spell.IsActive) spell.OnTriggerEnterCallBack(other);
    }
    void OnTriggerExit(Collider other)
    {
        if (spell.IsActive) spell.OnTriggerExitCallBack(other);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (spell.IsActive) spell.OnCollisionEnterCallBack(collision);
    }

}
