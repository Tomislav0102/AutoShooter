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
    [ReadOnly] public Faction myFaction = Faction.Neutral;

    [SerializeField] Element element;
    public bool canBeBlocked;
    public float areaOfEffect = 1;
    [SerializeField] protected float speed;
    [Range(0, 20)][SerializeField] protected int knockBack;
    [SerializeField] protected float lifeTime;
    [SerializeField] GameObject afterEffect;
    public Transform anchor;
    [SerializeField] protected DamageData damData;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    float _timerLife;
    bool _endDelayStarted;

    
    public virtual void InitializeMe(Brain brain)
    {
        _ownersBrain = brain;
        myFaction = _ownersBrain.faction;

        comp.myRigid.isKinematic = true;

        if (comp.mySphereCollider != null)
        {
            Physics.IgnoreCollision(comp.mySphereCollider, _ownersBrain.myCollider);
            comp.mySphereCollider.enabled = false;
            comp.mySphereCollider.radius = areaOfEffect * 0.5f;
        }
        if (comp.myCapsuleCollider != null)
        {
            Physics.IgnoreCollision(comp.myCapsuleCollider, _ownersBrain.myCollider);
            comp.myCapsuleCollider.enabled = false;
            comp.myCapsuleCollider.height = areaOfEffect;
            comp.myCapsuleCollider.center = areaOfEffect * 0.5f * Vector3.forward;
        }
    }

    public virtual void InitializeMe(Brain brain, float dam)
    {
        _ownersBrain = brain;
        damData.damage = dam;

        InitializeMe(brain);
    }

    public virtual void InitializeMe(Brain brain, float dam, HashSet<Collider> collidersToIgnore)
    {
        foreach (Collider col in collidersToIgnore) collidersDetected.Add(col);
        InitializeMe(brain, dam);
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
            spell.InitializeMe(_ownersBrain);
        }
    }
    public void OnEnd()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || !Ga.me.spells.showDebug) return;
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(comp.myTransform.position, areaOfEffect);
    }
}
