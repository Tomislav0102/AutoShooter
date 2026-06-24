using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;


public class Spell : SerializedMonoBehaviour
{
    protected bool initialized;
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
                    if (_timerPhase < warningDelay) main.warningRend.enabled = true;
                    break;
                case Phase.SpellRuns:
                    _timerPhase = 0f;
                    main.warningRend.enabled = false;
                    main.visual.PlayDefault();
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
    
    [SerializeField] protected FactionToTarget myFactionTarget = FactionToTarget.Enemy;
    public float areaOfEffect = 1;
    [SerializeField] float warningDelay;
    [Range(0f, 1f)] public float hitChance = 1f;
    [InfoBox("Lifetime info: -0 Endless | 0 Instant | +0 Specific")]
    public float lifeTime;
    bool LifeTimeIs0() => lifeTime == 0f;
    [HideIf(nameof(LifeTimeIs0))] public float rateOfFire;
    [Tooltip("if false it will play for 'lifetime' seconds. Does nothing if 'lifetime' == 0.")]
    [SerializeField, HideIf(nameof(LifeTimeIs0))] bool terminateOnHit = true;
    
    [SerializeField, BoxGroup] bool useInspectorDamageData;
    [SerializeField, ShowIf(nameof(useInspectorDamageData)), BoxGroup] protected InjectHealth injectHealth;
    //need this, because dictionary can't be serialized in non-monobehaviour C# class
    [SerializeField, ShowIf(nameof(useInspectorDamageData)), BoxGroup] Dictionary<Element, float> _damageInspector;
    
    protected enum HitEffect { Damage, Nullify, Reflect, StatChange }
    [SerializeField] protected List<HitEffect> hitEffects;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();


    
    public virtual void InitializeMe(SpellMain mainSpell)
    {
        main = mainSpell;
        if (!useInspectorDamageData) injectHealth = main.injectHealthPass;
        else injectHealth.damage = _damageInspector;
        injectHealth.myBrain = main.OwnersBrain;
        
        main.warningRend.transform.localScale = areaOfEffect * Vector3.one;
        main.mySphereCollider.radius = areaOfEffect * 0.5f;
        main.myCapsuleCollider.height = areaOfEffect;
        main.myCapsuleCollider.center = areaOfEffect * 0.5f * Vector3.forward;
        MyPhase = Phase.BeginWarning;
        initialized = true;
    }

    public virtual void OnCollisionEnterCallBack(Collision collision) { }
    public virtual void OnTriggerExitCallBack(Collider other) { }
    public virtual void OnTriggerEnterCallBack(Collider other) { }

    protected void HitMethod(Collider colliderHit, out Brain collidersBrain, SpellMain[] spellsToAffect = null)
    {
        Brain b = null;
        if (colliderHit.TryGetComponent(out Brain targetBrain) && Utils.CanTargetFaction(main.OwnersBrain.Faction, targetBrain.Faction, myFactionTarget))
        {
            if (injectHealth.knockBack > 0 && targetBrain.loco != null)
            {
                Vector3 dir = Utils.Direction(main.myTransform.position, targetBrain.myTransform.position);
                targetBrain.loco.KnockBack(dir, injectHealth.knockBack);
                main.onHitTarget?.Invoke(targetBrain);
                b = targetBrain;
            }

            if (hitEffects.Contains(HitEffect.Damage) && injectHealth.damage.Count > 0)
            {
                targetBrain.health.TakeDamage(injectHealth);
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
        if (!initialized) return;
        if (!main.IsActive) return;
        switch (MyPhase)
        {
            case Phase.BeginWarning:
                if (_timerPhase >= warningDelay) MyPhase = Phase.SpellRuns;
                break;
            case Phase.SpellRuns:
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















