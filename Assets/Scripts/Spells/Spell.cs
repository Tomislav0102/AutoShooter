using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Spell : EventBus
{
    [Title("References")]
    public Transform myTransform;
    public Rigidbody myRigid;
    public SphereCollider myCollider;
    public Transform myMesh;
    public Transform myVisualization;

    [Title("Data")]
    public Faction faction = Faction.Neutral;
    public Element element;
    public float damage;
    public float radius;
    public float speed;
    public float lifeTime;

    protected DamageData dam;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    float _timerLife;


    public virtual void InitializeMe(Brain brain) 
    {
        faction = brain.faction;
        myCollider.enabled = false;
        myRigid.isKinematic = true;
        myCollider.radius = radius;
        if (myVisualization != null) myVisualization.localScale = radius * 2 * Vector3.one;
        dam = new DamageData()
        {
            damage = damage,
            attacker = brain.loco.myTransform,
            element = element,
        };
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
