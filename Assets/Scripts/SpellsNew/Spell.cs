using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class Spell : SerializedMonoBehaviour
{
    protected SpellControl main;
    [ReadOnly] public Faction myFaction;
    [SerializeField] protected FactionToTarget myFactionTarget = FactionToTarget.Enemy;
    public float areaOfEffect = 1;
    [SerializeField] float startDelay;
    [SerializeField] protected float lifeTime;
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


    public virtual void InitializeMe(SpellControl mainSpell)
    {
        main = mainSpell;
        myFaction = main.ownersBrain.faction;
        injectHealthData.damage = useInspectorDamageData ? inspectorDamage : main.damage;
        
        main.myRigid.isKinematic = true;
        main.visualization.localScale = areaOfEffect * Vector3.one;
        main.warningRend.transform.localScale = areaOfEffect * Vector3.one;
        
        if (main.mySphereCollider != null)
        {
            Physics.IgnoreCollision(main.mySphereCollider, main.ownersBrain.myCollider);
            main.mySphereCollider.enabled = false;
            main.mySphereCollider.radius = areaOfEffect * 0.5f;
        }
        if (main.myCapsuleCollider != null) 
        {
            Physics.IgnoreCollision(main.myCapsuleCollider, main.ownersBrain.myCollider);
            main.myCapsuleCollider.enabled = false;
            main.myCapsuleCollider.height = areaOfEffect;
            main.myCapsuleCollider.center = areaOfEffect * 0.5f * Vector3.forward;
        }

        switch (colliderType)
        {
            case ColliderType.Sphere:
                main.mySphereCollider.enabled = true;
                break;
            case ColliderType.Capsule:
                main.myCapsuleCollider.enabled = true;
                break;
        }
        
        if (startDelay > 0) main.warningRend.enabled = true;
        timerStartDelay = new MyTimer(startDelay, () =>
        {
            main.warningRend.enabled = false;
        });
        
        main.onTrigEnter += CallEv_OnTriggerEnter;
        main.onTrigExit += CallEv_OnTriggerExit;
        main.onCollisionEnter += CallEv_OnCollisionEnter;
    }

    protected virtual void CallEv_OnCollisionEnter(Collision collision) { }
    protected virtual void CallEv_OnTriggerExit(Collider other) { }
    protected virtual void CallEv_OnTriggerEnter(Collider other) { }


    protected virtual void Update()
    {
        // timerStartDelay.UpdateLoop();
        // if (!timerStartDelay.completed) return;
        if (anchor != null) main.myTransform.position = anchor.position;

        _timerLife += Time.deltaTime;
        if (!_endDelayStarted && _timerLife >= lifeTime) StartCoroutine(DelayEnd());
    }

    IEnumerator DelayEnd()
    {
        _endDelayStarted = true;
        yield return new WaitForFixedUpdate();
        OnEnd();
    }


    public void OnEnd()
    {
        main.onTrigEnter -= CallEv_OnTriggerEnter;
        main.onTrigExit -= CallEv_OnTriggerExit;
        main.onCollisionEnter -= CallEv_OnCollisionEnter;

        Destroy(main.gameObject);
    }

}
