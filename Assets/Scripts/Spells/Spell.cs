using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Spell : EventBus
{
    [Title("References")]
    public Transform myTransform;
    public Rigidbody myRigid;
    public Collider myCollider;

    [Title("Data")]
    public Faction faction = Faction.Neutral;
    public Element element;
    public float damage;
    public float attackRange; //only for AI, switching weapons melee-ranged
    public float areaOfEffect;
    public float speed;
    public float lifeTime;

    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    float _timerLife;


    public virtual void InitializeMe(Faction fac) 
    {
        faction = fac;
        myCollider.enabled = false;
        myRigid.isKinematic = true;
        myTransform.localScale = areaOfEffect * Vector3.one;
    }
    
    protected void Update()
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
