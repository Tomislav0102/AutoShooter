using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Spell : EventBus
{
    [Title("References")]
    protected Brain brainCaster;
    public Transform myTransform;
    [SerializeField] protected Rigidbody myRigid;
    [SerializeField] protected SphereCollider myCollider;
    public Transform myMesh;
    [SerializeField] protected Transform myVisualization;

    [Title("Data")]
    [SerializeField] protected Faction faction = Faction.Neutral;
    [SerializeField] Element element;
    [SerializeField] protected float damage;
    public float radius;
    [SerializeField] float speed;
    [Range(0, 20)] [SerializeField] protected int knockBack;
    [SerializeField] protected float lifeTime;
    [SerializeField] protected GameObject afterEffect;
    protected DamageData dam;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    float _timerLife;


    public virtual void InitializeMe(Brain brain) 
    {
        brainCaster = brain;
        faction = brainCaster.faction;
        myCollider.enabled = false;
        myRigid.isKinematic = true;
        myCollider.radius = radius;
        if (myVisualization != null) myVisualization.localScale = radius * 2 * Vector3.one;
        dam = new DamageData()
        {
            damage = damage,
            knockBack = knockBack,
            attacker = this.brainCaster.myTransform,
            element = element,
        };
        Physics.IgnoreCollision(myCollider, this.brainCaster.myCollider);
    }
    
    /// <summary>
    /// Debug only
    /// </summary>
    // public virtual void InitializeMe(Transform attacker, Faction fac) 
    // {
    //     faction =fac;
    //     //myCollider.enabled = false;
    //     myRigid.isKinematic = true;
    //     myCollider.radius = radius;
    //     dam = new DamageData()
    //     {
    //         damage = damage,
    //         attacker = attacker,
    //         element = element,
    //     };
    // }
    
    protected virtual void Update()
    {
        if (lifeTime > 0)
        {
            _timerLife += Time.deltaTime;
            if (_timerLife >= lifeTime) StartCoroutine(Delay());
        }
    }
    IEnumerator Delay()
    {
        yield return new WaitForFixedUpdate();
        OnEnd();
    }

    protected void SetSpeed()
    {
        myRigid.linearVelocity = speed * myTransform.forward;
    }

    protected virtual void OnEnd()
    {

        Destroy(gameObject);
    }
}
