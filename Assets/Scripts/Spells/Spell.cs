using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;


public class Spell :SerializedMonoBehaviour
{
    [ReadOnly] public CompSpellContainer container;
    [ReadOnly] public Faction myFaction;
    [SerializeField] protected FactionToTarget myFactionTarget = FactionToTarget.Enemy;
    public float areaOfEffect = 1;
    [SerializeField] float startDelay;
    [SerializeField] protected float speed;
    [SerializeField] protected float lifeTime;
    [SerializeField] Spell afterEffect;
    [ReadOnly] public Transform anchor;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    float _timerLife;
    bool _endDelayStarted;
    protected MyTimer timerStartDelay;
    protected enum ColliderType { Sphere, Capsule, None }
    [SerializeField] protected ColliderType colliderType;
    [SerializeField, BoxGroup("Particles", false)] protected SpellParticles spellParticles;
    
    [SerializeField, BoxGroup] protected InjectHealth injectHealthData;
    [SerializeField, BoxGroup] protected bool useInspectorDamageData;
    [SerializeField, ShowIf(nameof(useInspectorDamageData)), BoxGroup] protected Dictionary<Element, float> inspectorDamage =  new Dictionary<Element, float>();


    public void InitializeMeShared(CompSpellContainer cont)
    {
        container = cont;
        myFaction = container.ownersBrain.faction;
        if (useInspectorDamageData || injectHealthData.damage == null) injectHealthData.damage = inspectorDamage;
        
        container.comp.myRigid.isKinematic = true;
        container.comp.visualization.localScale = areaOfEffect * Vector3.one;
        container.comp.warningRend.transform.localScale = areaOfEffect * Vector3.one;
        
        if (container.comp.mySphereCollider != null)
        {
            Physics.IgnoreCollision(container.comp.mySphereCollider, container.ownersBrain.myCollider);
            container.comp.mySphereCollider.enabled = false;
            container.comp.mySphereCollider.radius = areaOfEffect * 0.5f;
        }
        if (container.comp.myCapsuleCollider != null) //not used
        {
            Physics.IgnoreCollision(container.comp.myCapsuleCollider, container.ownersBrain.myCollider);
            container.comp.myCapsuleCollider.enabled = false;
            container.comp.myCapsuleCollider.height = areaOfEffect;
            container.comp.myCapsuleCollider.center = areaOfEffect * 0.5f * Vector3.forward;
        }

        switch (colliderType)
        {
            case ColliderType.Sphere:
                container.comp.mySphereCollider.enabled = true;
                break;
            case ColliderType.Capsule:
                container.comp.myCapsuleCollider.enabled = true;
                break;
        }
        
        if (startDelay > 0) container.comp.warningRend.enabled = true;
        timerStartDelay = new MyTimer(startDelay, () =>
        {
            container.comp.warningRend.enabled = false;
        });
    }




    protected virtual void Update()
    {
        // if (startDelay > 0)
        // {
        //     startDelay -= Time.deltaTime;
        //     container.compSpell.warningRend.enabled = true;
        //     return;
        // }
        // container.compSpell.warningRend.enabled = false;
        timerStartDelay.UpdateLoop();
        if (!timerStartDelay.completed) return;
        if (anchor != null) container.comp.myTransform.position = anchor.position;

        _timerLife += Time.deltaTime;
        if (!_endDelayStarted && _timerLife >= lifeTime) StartCoroutine(DelayEnd());
    }

    IEnumerator DelayEnd()
    {
        _endDelayStarted = true;
        yield return new WaitForFixedUpdate();
        OnEnd();
    }

    protected void SetSpeed()
    {
        container.comp.myRigid.linearVelocity = speed * container.comp.myTransform.forward;
    }

    protected void AfterEffect()
    {
        // if (afterEffect == null) return;
        // Spell spell = Instantiate(afterEffect, container.compSpell.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        // spell.InitializeMeShared();
    }

    public void OnEnd()
    {
        Destroy(gameObject);
    }

}
