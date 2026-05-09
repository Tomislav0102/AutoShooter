using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class Spell : SerializedMonoBehaviour
{
    public enum Phase
    {
        BeginWarning,
        SpellRuns,
        EndStart,
        EndEnd,
    }
    public Phase MyPhase
    {
        get => _phase;
        set
        {
            _phase = value;
            switch (value)
            {
                case Phase.BeginWarning:
                    if (_timerPhase < startDelay) main.warningRend.enabled = true;
                    break;
                case Phase.SpellRuns:
                    _timerPhase = 0f;
                    main.warningRend.enabled = false;
                    break;
                case Phase.EndStart:
                    MyPhase = Phase.EndEnd;
                    main.onEnd?.Invoke();
                    break;
            }
        }
    }
    [ShowInInspector, ReadOnly] Phase _phase;
    float _timerPhase;
    
    protected SpellControl main;
    [ReadOnly] public Faction myFaction;
    [SerializeField] protected FactionToTarget myFactionTarget = FactionToTarget.Enemy;
    public float areaOfEffect = 1;
    [SerializeField] float startDelay;
    [SerializeField] protected float lifeTime;
    [ReadOnly] public Transform anchor;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
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
        injectHealthData.attacker = main.ownersBrain.myTransform;
        
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

        MyPhase = Phase.BeginWarning;
        
        main.onTrigEnter += CallEv_OnTriggerEnter;
        main.onTrigExit += CallEv_OnTriggerExit;
        main.onCollisionEnter += CallEv_OnCollisionEnter;
        main.onEnd += CallEv_End;
    }

    void CallEv_End()
    {
        main.onTrigEnter -= CallEv_OnTriggerEnter;
        main.onTrigExit -= CallEv_OnTriggerExit;
        main.onCollisionEnter -= CallEv_OnCollisionEnter;
    }

    protected virtual void CallEv_OnCollisionEnter(Collision collision) { }
    protected virtual void CallEv_OnTriggerExit(Collider other) { }
    protected virtual void CallEv_OnTriggerEnter(Collider other) { }


    protected virtual void Update()
    {
        switch (MyPhase)
        {
            case Phase.BeginWarning:
                if (_timerPhase >= startDelay) MyPhase = Phase.SpellRuns;
                break;
            case Phase.SpellRuns:
                if (anchor != null) main.myTransform.position = anchor.position;
                if (_timerPhase > lifeTime) MyPhase = Phase.EndStart;
                break;
        }
        _timerPhase += Time.deltaTime;
    }

}















