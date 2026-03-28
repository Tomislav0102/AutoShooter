using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Spell : EventBus
{
    [Title("References")]
    Brain _ownersBrain;
    public Transform myTransform;
    [SerializeField] protected Rigidbody myRigid;
    public SphereCollider myCollider;
    public Transform myMesh;
    [SerializeField] protected Transform myVisualization;

    [Title("Data")]
    [SerializeField] protected Faction faction = Faction.Neutral;
    [SerializeField] Element element;
    [SerializeField] protected float damage;
    [SerializeField] protected bool canBeBlocked;
    public float radius;
    [SerializeField] protected float speed;
    [Range(0, 20)] [SerializeField] protected int knockBack;
    [SerializeField] protected float lifeTime;
    [SerializeField] GameObject afterEffect;
    public Transform anchor;
    protected DamageData dam;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    float _timerLife;


    public virtual void InitializeMe(Brain brain) 
    {
        _ownersBrain = brain;
        faction = _ownersBrain.faction;
        myCollider.enabled = false;
        myRigid.isKinematic = true;
        myCollider.radius = radius;
        if (myVisualization != null) myVisualization.localScale = radius * 2 * Vector3.one;
        dam = new DamageData()
        {
            attacker = _ownersBrain.myTransform,
            damage = damage,
            canBeBlocked = canBeBlocked,
            knockBack = knockBack,
            element = element,
        };
        Physics.IgnoreCollision(myCollider, _ownersBrain.myCollider);
    }
    
    protected virtual void Update()
    {
        if (lifeTime > 0)
        {
            _timerLife += Time.deltaTime;
            if (_timerLife >= lifeTime) StartCoroutine(Delay());
        }
        if (anchor != null) myTransform.position = anchor.position;
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

    protected void AfterEffect()
    {
        if (afterEffect != null)
        {
            Spell spell = Instantiate(afterEffect, myTransform.position, Quaternion.identity, Ga.me.spells.myTransform).GetComponent<Spell>();
            spell.InitializeMe(_ownersBrain);
        }
    }
    protected virtual void OnEnd()
    {

        Destroy(gameObject);
    }
}
