using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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
    public float areaOfEffect = 1f;
    [SerializeField] float warningDelay;
    [Range(0f, 1f)] public float hitChance = 1f;
    [InfoBox("Lifetime info: -0 Endless | 0 Instant | +0 Specific")]
    public float lifeTime;
    bool LifeTimeIs0() => lifeTime == 0f;
    [HideIf(nameof(LifeTimeIs0))] public float rateOfFire;
    [Tooltip("if false it will play for 'lifetime' seconds. Does nothing if 'lifetime' == 0.")]
    [SerializeField, HideIf(nameof(LifeTimeIs0))] bool terminateOnHit = true;
    PassDataContainer _pd;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();


    public virtual void InitializeMe(SpellMain mainSpell)
    {
        main = mainSpell;
        main.warningRend.transform.localScale = areaOfEffect * Vector3.one;
        main.mySphereCollider.radius = areaOfEffect * 0.5f;
        main.myCapsuleCollider.height = areaOfEffect;
        main.myCapsuleCollider.center = areaOfEffect * 0.5f * Vector3.forward;
        
        OvrPassData ovrPassData = GetComponent<OvrPassData>();
        if (ovrPassData != null) _pd = ovrPassData.GetContainer();
        else _pd = mainSpell.pd;
        _pd.myBrain = main.OwnersBrain;
        
        MyPhase = Phase.BeginWarning;
        initialized = true;
        
    }

    public virtual void OnCollisionEnterCallBack(Collision collision) { }
    public virtual void OnTriggerExitCallBack(Collider other) { }
    public virtual void OnTriggerEnterCallBack(Collider other) { }


    protected void HitGeneric<T>(T targetGeneric, out Brain targetsBrain) where T : Component
    {
        Brain oustedTargetsBrain = null;
        foreach (PassData item in _pd.data)
        {
            switch (item)
            {
                case PassDataDamage dam:
                case PassDataKnockBack knockBack:
                case PassDataManaShield manaShield:
                    if (targetGeneric.TryGetComponent(out Brain br) && 
                        Utils.CanTargetFaction(main.OwnersBrain.Faction, br.Faction, myFactionTarget))
                    {
                        br.health.TakeDamage(_pd);
                        oustedTargetsBrain = br;
                    }
                    break;
                
                case PassDataSpell spellData:
                    if (targetGeneric.TryGetComponent(out SpellMain targetSpell) &&
                        Utils.CanTargetFaction(main.OwnersBrain.Faction, targetSpell.OwnersBrain.Faction, myFactionTarget))
                    {
                        if (spellData.spellsToAffect.Length == 0) onSpell();
                        else
                        {
                            for (int i = 0; i < spellData.spellsToAffect.Length; i++)
                            {
                                if (targetSpell.spell.GetType() != spellData.spellsToAffect[i].spell.GetType()) continue;
                                onSpell();
                            }
                        }
                        void onSpell()
                        {
                            switch (spellData.effect)
                            {
                                case PassData.HitEffectOnSpell.Nullify:
                                    targetSpell.spell.MyPhase = Phase.EndStart;
                                    break;
                                case PassData.HitEffectOnSpell.Reflect:
                                    Vector3 newDirection = Utils.Direction(main.myTransform.position, targetSpell.myTransform.position);
                                    targetSpell.transporter.ReflectProjectile(main.OwnersBrain, newDirection);
                                    break;
                            }
                        }
                    }
                    break;
            }
        }
        targetsBrain = oustedTargetsBrain;
    }



    protected virtual void Update()
    {
        if (!initialized) return;
        if (!main.mainActive) return;
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
                    StartCoroutine(delay());
                    return;
                    IEnumerator delay()
                    {
                        yield return new WaitForFixedUpdate();
                        MyPhase = Phase.EndEnd;
                    }
                }
                if (_timerPhase > lifeTime) MyPhase = Phase.EndEnd;
                break;
        }
        _timerPhase += Time.deltaTime;
    }

}
















