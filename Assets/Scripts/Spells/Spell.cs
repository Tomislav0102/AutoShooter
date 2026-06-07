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
    [ReadOnly] public Transform followTarget;
    protected SpellMain main;
    
    [SerializeField] protected FactionToTarget myFactionTarget = FactionToTarget.Enemy;
    public float areaOfEffect = 1;
    [SerializeField] float startDelay;
    [Range(0f, 1f)] public float hitChance = 1f;
    [InfoBox("Lifetime info: -0 Endless | 0 Instant | +0 Specific")]
    public float lifeTime;
    public float rateOfFire;
    [Tooltip("if false it will play for 'lifetime' seconds. Does nothing if 'lifetime' == 0.")]
    [SerializeField] bool terminateOnHit = true;
    
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

    public virtual void OnCollisionEnterCallBack(Collision collision) { }
    public virtual void OnTriggerExitCallBack(Collider other) { }
    public virtual void OnTriggerEnterCallBack(Collider other) { }

    protected void HitMethod(Collider colliderHit, out Brain collidersBrain, SpellMain[] spellsToAffect = null)
    {
        Brain b = null;
        if (colliderHit.TryGetComponent(out Brain targetBrain) && Utils.CanTargetFaction(main.OwnersBrain.Faction, targetBrain.Faction, myFactionTarget))
        {
            
            if (injectHealthData.knockBack > 0 && targetBrain.loco != null)
            {
                Vector3 dir = Utils.Direction(main.myTransform.position, targetBrain.myTransform.position);
                targetBrain.loco.KnockBack(dir, injectHealthData.knockBack);
                main.onHitTarget?.Invoke(targetBrain);
                b = targetBrain;
            }

            if (hitEffects.Contains(HitEffect.Damage) && injectHealthData.damage.Count > 0)
            {
                targetBrain.health.TakeDamage(injectHealthData);
                b = targetBrain;
            }

            if (!collidersDetected.Contains(colliderHit))
            {
                collidersDetected.Add(colliderHit);
                if (hitEffects.Contains(HitEffect.StatChange))
                {
                    //change stats
                }
            }
        }
        collidersBrain = b;

        if (colliderHit.TryGetComponent(out SpellMain targetSpell) && Utils.CanTargetFaction(main.OwnersBrain.Faction, targetSpell.OwnersBrain.Faction, myFactionTarget))
        {
            if (spellsToAffect == null) return;
            for (int i = 0; i < spellsToAffect.Length; i++)
            {
                if (targetSpell.spell.GetType() != spellsToAffect[i].spell.GetType()) continue;
                main.onHitTarget?.Invoke(targetSpell.OwnersBrain); //might not work

                if (hitEffects.Contains(HitEffect.Nullify)) targetSpell.spell.MyPhase = Phase.EndStart;

                if (hitEffects.Contains(HitEffect.Reflect))
                {
                    Vector3 newDirection = Utils.Direction(main.myTransform.position, targetSpell.myTransform.position);
                    targetSpell.transporter.ReflectProjectile(main.OwnersBrain, newDirection);
                }
            }

        }
    }


    protected virtual void Update()
    {
        if (!main.IsActive) return;
        switch (MyPhase)
        {
            case Phase.BeginWarning:
                if (_timerPhase >= startDelay) MyPhase = Phase.SpellRuns;
                break;
            case Phase.SpellRuns:
                if (followTarget != null) main.myTransform.position = followTarget.position;
                if (lifeTime < 0) return;
                if (lifeTime == 0)
                {
                    lifeTime = -1;
                    StartCoroutine(Delay());
                    IEnumerator Delay()
                    {
                        yield return new WaitForFixedUpdate();
                        MyPhase = Phase.EndEnd;
                    }
                    return;
                }
                if (_timerPhase > lifeTime) MyPhase = Phase.EndEnd;
                break;
        }
        _timerPhase += Time.deltaTime;
    }

}















