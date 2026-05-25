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
                    if (terminateOnHit) MyPhase = Phase.EndEnd;
                    else MyPhase = Phase.SpellRuns;
                    break;
                case Phase.EndEnd:
                    main.onEnd?.Invoke();
                    break;
            }
        }
    }
    [ShowInInspector, ReadOnly] Phase _phase;
    float _timerPhase;
    
    protected SpellMain main;
  //  [ReadOnly] public Faction myFaction;
    [SerializeField] protected FactionToTarget myFactionTarget = FactionToTarget.Enemy;
    public float areaOfEffect = 1;
    [SerializeField] float startDelay;
    protected enum DurationType { Instant, Endless, Specific }
    [SerializeField] protected DurationType durationType = DurationType.Instant;
    bool ShowLifeTime() => durationType == DurationType.Specific;
    [ShowIf(nameof(ShowLifeTime))] public float lifeTime;
    [SerializeField] bool terminateOnHit = true;
    [ReadOnly] public Transform followTarget;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    [SerializeField, BoxGroup("Particles", false)] 
    protected SpellParticles spellParticles;
    
    [SerializeField, BoxGroup] protected InjectHealth injectHealthData;
    [SerializeField, BoxGroup] bool useInspectorDamageData;
    [SerializeField, ShowIf(nameof(useInspectorDamageData)), BoxGroup] 
    Dictionary<Element, float> inspectorDamage =  new Dictionary<Element, float>();
    protected enum HitEffect { Damage, Nullify, Reflect, StatChange }
    [SerializeField] protected List<HitEffect> hitEffects;


    public virtual void InitializeMe(SpellMain mainSpell)
    {
        main = mainSpell;
        injectHealthData.damage = useInspectorDamageData ? inspectorDamage : main.damage;
        injectHealthData.myBrain = main.OwnersBrain;
        main.myRigid.isKinematic = true;
        main.visualization.localScale = areaOfEffect * Vector3.one;
        main.warningRend.transform.localScale = areaOfEffect * Vector3.one;
        main.mySphereCollider.radius = areaOfEffect * 0.5f;
        main.myCapsuleCollider.height = areaOfEffect;
        main.myCapsuleCollider.center = areaOfEffect * 0.5f * Vector3.forward;
        
        MyPhase = Phase.BeginWarning;
    }

    public virtual void CallEv_OnCollisionEnter(Collision collision) { }
    public virtual void CallEv_OnTriggerExit(Collider other) { }
    public virtual void CallEv_OnTriggerEnter(Collider other) { }


    protected virtual void Update()
    {
        switch (MyPhase)
        {
            case Phase.BeginWarning:
                if (_timerPhase >= startDelay) MyPhase = Phase.SpellRuns;
                break;
            case Phase.SpellRuns:
                if (followTarget != null) main.myTransform.position = followTarget.position;
                if (durationType == DurationType.Specific && _timerPhase > lifeTime) MyPhase = Phase.EndEnd;
                break;
        }
        _timerPhase += Time.deltaTime;
    }

}















