using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class SpellMain : MonoBehaviour
{
    public Transform myTransform;
    public Rigidbody myRigid;
    public SphereCollider mySphereCollider;
    public CapsuleCollider myCapsuleCollider;
    public Transform myMesh;
    public SpriteRenderer warningRend;
    public Transform visualization;
    [ReadOnly] public Brain ownersBrain;
    [ReadOnly] public Dictionary<Element, float> damage = new Dictionary<Element, float>();
    public Spell spell; 
    public SpellTransporter transporter;
    [SerializeField, TextArea, HideLabel] string description;

    public System.Action<Collider> onTrigEnter;
    public System.Action<Collider> onTrigExit;
    public System.Action<Collision> onCollisionEnter;
    public System.Action onEnd;
    System.Action _onAfterEffect;

    /// <summary>
    /// Damage is from inspector, no after effect
    /// </summary>
    public void InitializeMe(Brain brain)
    {
        ownersBrain = brain;
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

    void CallEv_OnEnd()
    {
        _onAfterEffect?.Invoke();
        Destroy(gameObject);
    }
    
    void OnTriggerEnter(Collider other) => onTrigEnter?.Invoke(other);
    void OnTriggerExit(Collider other) => onTrigExit?.Invoke(other);
    void OnCollisionEnter(Collision collision) => onCollisionEnter?.Invoke(collision);

}
