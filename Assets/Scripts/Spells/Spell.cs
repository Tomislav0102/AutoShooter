using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class SpellData
{
    public Faction faction = Faction.Neutral;
    public Element element;
    public float damage;
    public float attackRange; //only for AI, switching weapons melee-ranged
    public float areaOfEffect;
    public float speed;
    public float lifeTime;

}
public class Spell : EventBus
{
    public SpellData myData;
    [HideInInspector] public Transform myTransform;
    protected Rigidbody myRigid;
    protected Collider myCollider;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    float _timerLife;


    public virtual void InitializeMe(SpellData dat) 
    {
        myData = dat;
        InitializeMe();
    }
    public virtual void InitializeMe(Faction fac) 
    {
        myData.faction = fac;
        InitializeMe();
    }
    protected virtual void InitializeMe() //if spellData is from inspector
    {
        myTransform = transform;
        myRigid = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        myCollider.enabled = false;
        myRigid.isKinematic = true;
        myTransform.localScale = myData.areaOfEffect * Vector3.one;
    }
    protected void Update()
    {
        if (myData.lifeTime > 0)
        {
            _timerLife += Time.deltaTime;
            if (_timerLife >= myData.lifeTime) StartCoroutine(Delay());
        }
    }
    IEnumerator Delay()
    {
        yield return new WaitForFixedUpdate();
        OnEnd();
    }

    protected void SetSpeed()
    {
        myRigid.linearVelocity = myData.speed * myTransform.forward;
    }
    protected virtual void OnEnd()
    {
        Destroy(gameObject);
    }
}
