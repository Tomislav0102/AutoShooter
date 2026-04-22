using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class Spell : EventBus
{
    Brain _ownersBrain;
    public CompSpell comp;
    [ReadOnly] public Faction myFaction;
    [SerializeField] protected FactionToTarget myFactionTarget = FactionToTarget.Enemy;
    public float areaOfEffect = 1;
    [SerializeField] protected float speed;
    [SerializeField] protected float lifeTime;
    [SerializeField] GameObject afterEffect;
    public Transform anchor;
    [SerializeField, BoxGroup] protected InjectHealth injectHealthData;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    float _timerLife;
    bool _endDelayStarted;


    void InitializeMeShared(Brain brain)
    {
        _ownersBrain = brain;
        myFaction = _ownersBrain.faction;

        comp.myRigid.isKinematic = true;
        comp.visualization.localScale = areaOfEffect * Vector3.one;
        if (comp.mySphereCollider != null)
        {
            Physics.IgnoreCollision(comp.mySphereCollider, _ownersBrain.myCollider);
            comp.mySphereCollider.enabled = false;
            comp.mySphereCollider.radius = areaOfEffect * 0.5f;
        }

        if (comp.myCapsuleCollider != null) //not used
        {
            Physics.IgnoreCollision(comp.myCapsuleCollider, _ownersBrain.myCollider);
            comp.myCapsuleCollider.enabled = false;
            comp.myCapsuleCollider.height = areaOfEffect;
            comp.myCapsuleCollider.center = areaOfEffect * 0.5f * Vector3.forward;
        }
    }

    public virtual void InitializeMe(Brain brain)
    {
        InitializeMeShared(brain);
    }

    public virtual void InitializeMe(Brain brain, Dictionary<Element, float> damage)
    {
        injectHealthData.damage = damage;

        InitializeMeShared(brain);
    }


    protected virtual void Update()
    {
        if (anchor != null) comp.myTransform.position = anchor.position;

        _timerLife += Time.deltaTime;
        if (!_endDelayStarted && _timerLife >= lifeTime) StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        _endDelayStarted = true;
        yield return new WaitForFixedUpdate();
        OnEnd();
    }

    protected void SetSpeed()
    {
        comp.myRigid.linearVelocity = speed * comp.myTransform.forward;
    }

    protected void AfterEffect()
    {
        if (afterEffect != null)
        {
            Spell spell = Instantiate(afterEffect, comp.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform).GetComponent<Spell>();
            spell.InitializeMeShared(_ownersBrain);
        }
    }

    public void OnEnd()
    {
        Destroy(gameObject);
    }

}
