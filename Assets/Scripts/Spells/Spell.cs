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
    
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    
    protected enum HitEffect { OnHealth, OnSpell, OnStats, OnShield }
    protected enum HitEffectOnSpell { Nullify, Reflect }
    [SerializeField] protected HitEffect hitEffect;
    bool AffectsSpells() => hitEffect == HitEffect.OnSpell;
    [SerializeField, ShowIf(nameof(AffectsSpells))] protected  HitEffectOnSpell hitEffectOnSpell;
    //Only type matters. All instances of same type are treated the same. E.g., any 'S_Bullet' in array detects all variations. If array is empty that detects all.
    [SerializeField, ShowIf(nameof(AffectsSpells))] protected SpellMain[] spellsToAffect = System.Array.Empty<SpellMain>();

    
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


    protected void HitGeneric<T>(T targetGeneric, out Brain targetsBrain) where T : Component
    {
        Brain b = null;
        switch (hitEffect)
        {
            case HitEffect.OnHealth:
                if (targetGeneric.TryGetComponent(out Brain br) && Utils.CanTargetFaction(main.OwnersBrain.Faction, br.Faction, myFactionTarget))
                {
                    if (injectHealth.knockBack > 0 && br.loco != null)
                    {
                        Vector3 dir = Utils.Direction(main.myTransform.position, br.myTransform.position);
                        br.loco.KnockBack(dir, injectHealth.knockBack);
                        b = br;
                    }
                    if (injectHealth.damage.Count > 0)
                    {
                        br.health.TakeDamage(injectHealth);
                        b = br;
                    }
                }
                break;
            case HitEffect.OnSpell:
                if (targetGeneric.TryGetComponent(out SpellMain targetSpell) && Utils.CanTargetFaction(main.OwnersBrain.Faction, targetSpell.OwnersBrain.Faction, myFactionTarget))
                {
                    if (spellsToAffect.Length == 0) Method();
                    else
                    {
                        for (int i = 0; i < spellsToAffect.Length; i++)
                        {
                            if (targetSpell.spell.GetType() != spellsToAffect[i].spell.GetType()) continue;
                            Method();
                        }
                    }
                }

                void Method()
                {
                    switch (hitEffectOnSpell)
                    {
                        case HitEffectOnSpell.Nullify:
                            targetSpell.spell.MyPhase = Phase.EndStart;
                            break;
                        case HitEffectOnSpell.Reflect:
                            Vector3 newDirection = Utils.Direction(main.myTransform.position, targetSpell.myTransform.position);
                            targetSpell.transporter.ReflectProjectile(main.OwnersBrain, newDirection);
                            break;
                    }
                    b = targetSpell.OwnersBrain;
                }

                break;
            case HitEffect.OnShield: //only one spell uses this, consider different solution for shield logic. Too much of the edge-case
                if (targetGeneric.TryGetComponent(out Brain brShield) && 
                    Utils.CanTargetFaction(main.OwnersBrain.Faction, brShield.Faction, myFactionTarget) &&
                    injectHealth.damage.ContainsKey(Element.Physical))
                {
                    brShield.health.SetShield(injectHealth.damage[Element.Physical]);
                    b = brShield;
                }
                break;
        }
        targetsBrain = b;
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
                    lifeTime = Mathf.NegativeInfinity;
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





    // protected void HitMethod(Collider colliderHit, out Brain collidersBrain, SpellMain[] spellsAffected = null)
    // {
    //     Brain b = null;
    //     if (colliderHit.TryGetComponent(out Brain targetBrain) && Utils.CanTargetFaction(main.OwnersBrain.Faction, targetBrain.Faction, myFactionTarget))
    //     {
    //         if (injectHealth.knockBack > 0 && targetBrain.loco != null)
    //         {
    //             Vector3 dir = Utils.Direction(main.myTransform.position, targetBrain.myTransform.position);
    //             targetBrain.loco.KnockBack(dir, injectHealth.knockBack);
    //             b = targetBrain;
    //         }
    //
    //         switch (hitEffect)
    //         {
    //             case HitEffect.OnHealth:
    //                 if (injectHealth.damage.Count > 0)
    //                 {
    //                     targetBrain.health.TakeDamage(injectHealth);
    //                     b = targetBrain;
    //                 }
    //                 break;
    //             case HitEffect.OnStats:
    //                 if (!collidersDetected.Contains(colliderHit))
    //                 {
    //                     collidersDetected.Add(colliderHit);
    //                     //change stats
    //                 }
    //                 break;
    //         }
    //
    //     }
    //     collidersBrain = b;
    //
    //     if (colliderHit.TryGetComponent(out SpellMain targetSpell) && Utils.CanTargetFaction(main.OwnersBrain.Faction, targetSpell.OwnersBrain.Faction, myFactionTarget))
    //     {
    //         if (spellsAffected.Length == 0)
    //         {
    //             HitMethod_Continue();
    //         }
    //         for (int i = 0; i < spellsAffected.Length; i++)
    //         {
    //             if (targetSpell.spell.GetType() != spellsAffected[i].spell.GetType()) continue;
    //             HitMethod_Continue();            
    //         }
    //         
    //         void HitMethod_Continue()
    //         {
    //             switch (hitEffect)
    //             {
    //                 case HitEffect.OnHealth:
    //                     break;
    //                 // case HitEffect.Nullify:
    //                 //     targetSpell.spell.MyPhase = Phase.EndStart;
    //                 //     break;
    //                 // case HitEffect.Reflect:
    //                 //     Vector3 newDirection = Utils.Direction(main.myTransform.position, targetSpell.myTransform.position);
    //                 //     targetSpell.transporter.ReflectProjectile(main.OwnersBrain, newDirection);
    //                 //     break;
    //                 case HitEffect.OnStats:
    //                     break;
    //             }
    //         }
    //     }
    // }













