using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;


public class SpellMain : MonoBehaviour
{
    [BoxGroup] public bool debug;
    [Title("References")]
    public Transform myTransform;
    public Rigidbody myRigid;
    public ColliderType colliderType;
    public SphereCollider mySphereCollider;
    public CapsuleCollider myCapsuleCollider;
    public SpriteRenderer warningRend;
    [SerializeField] GameObject effectGo;
    public SpellTransporter transporter;
    public SpellVisual visual;

    [BoxGroup("id", false), ReadOnly] public int id;
    [BoxGroup("id", false)] [Button]
    void GenerateId()=> id = Random.Range(0, 999999999);
    
    [Title("Data")]
    public bool isInterrupt;
    [ReadOnly] public bool spellActive;
    [ReadOnly] public PassDataContainer pd;
    public Brain OwnersBrain
    {
        get => _ownersBrain;
        set
        {
            if (_ownersBrain != null)
            {
                Physics.IgnoreCollision(mySphereCollider, OwnersBrain.myCollider, false);
                Physics.IgnoreCollision(myCapsuleCollider, OwnersBrain.myCollider, false);
            }
            
            _ownersBrain = value;
            Physics.IgnoreCollision(mySphereCollider, OwnersBrain.myCollider, false);
            Physics.IgnoreCollision(myCapsuleCollider, OwnersBrain.myCollider, false);
            mySphereCollider.enabled = false;
            myCapsuleCollider.enabled = false;
            switch (colliderType)
            {
                case ColliderType.Sphere:
                    Physics.IgnoreCollision(mySphereCollider, OwnersBrain.myCollider);
                    mySphereCollider.enabled = true;
                    break;
                case ColliderType.Capsule:
                    Physics.IgnoreCollision(myCapsuleCollider, OwnersBrain.myCollider);
                    myCapsuleCollider.enabled = true;
                    break;
            }

        }
    }
    [ReadOnly, ShowInInspector] Brain _ownersBrain;
    [SerializeField] float warningDelay;
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
                    if (_timerPhase < warningDelay) warningRend.enabled = true;
                    break;
                case Phase.SpellRuns:
                    _timerPhase = 0f;
                    warningRend.enabled = false;
                    visual.PlayDefault();
                    break;
                case Phase.EndStart:
                    if (terminateOnHit) MyPhase = Phase.EndEnd;
                    else MyPhase = Phase.SpellRuns;
                    break;
                case Phase.EndEnd:
                    onEnd?.Invoke();
                    break;
            }
        }
    }
    [ShowInInspector, ReadOnly] Phase _phase;
    float _timerPhase;
    public FactionToTarget myFactionTarget = FactionToTarget.Enemy;
    [InfoBox("Range info: in melee transporter defines 'area of effect'. With enemies, mage and archer defines attack distance.")]
    public float range = 1f;
    public float areaOfEffect = 1f;
    [Range(0f, 1f)] public float hitChance = 1f;
    public bool extraLifeTimeForParticles;
    [InfoBox("Lifetime info: -0 Endless | 0 Instant | +0 Specific")]
    public float lifeTime;
    bool LifeTimeIs0() => lifeTime == 0f;
    [Tooltip("if false it will play for 'lifetime' seconds. Does nothing if 'lifetime' == 0.")]
    [SerializeField, HideIf(nameof(LifeTimeIs0))] bool terminateOnHit = true;
    public HashSet<Collider> collidersDetected = new HashSet<Collider>();

    [Title("Events")]
    [SerializeField] UnityEvent<Collider> onTrigEnter;
    [SerializeField] UnityEvent<Collider> onTrigExit;
    [SerializeField] UnityEvent<Collision> onCollEnter;


    #region INITIALIZATION
    /// <summary>
    /// Damage is from inspector, no after spell
    /// </summary>
    public void InitializeMe(Brain brain)
    {
        if (brain == null) //it's a hack, but it works
        {
            Destroy(gameObject);
            return;
        }
        if (id == 0) print($"{gameObject.name} ID is 0, need to assign ID in inspector!");
        OwnersBrain = brain;
        onEnd += CallEv_OnEnd;
        myRigid.isKinematic = true;
        if (isInterrupt) gameObject.layer = LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt);
        
        MeleeTransporter meleeTransporter = transporter as MeleeTransporter;
        if (meleeTransporter != null) areaOfEffect = range;
        warningRend.transform.localScale = areaOfEffect * Vector3.one;
        mySphereCollider.radius = areaOfEffect * 0.5f;
        myCapsuleCollider.height = areaOfEffect;
        myCapsuleCollider.center = areaOfEffect * 0.5f * Vector3.forward;
        
        OvrPassData ovrPassData = GetComponent<OvrPassData>();
        if (ovrPassData != null) pd = ovrPassData.GetContainer();
        if (pd == null)
        {
            print($"No PassData found, destroying {gameObject.name} spell.");
            Destroy(gameObject);
            return;
        }
        pd.myBrain = OwnersBrain;
        if (pd.data == null) pd.data = new List<PassData>();
        
        MyPhase = Phase.BeginWarning;
        spellActive = true;
        transporter.Spell = this;
        visual.Spell = this;
        IIniSpell[] effects = effectGo.GetComponents<IIniSpell>();
        foreach (IIniSpell item in effects)
        {
            item.Spell = this;
        }
    }
    /// <summary>
    /// Damage is from code, no after spell
    /// </summary>
    public void InitializeMe(Brain brain, PassDataContainer passData)
    {
        pd = passData;
        InitializeMe(brain); 
    }
    /// <summary>
    /// Damage is from inspector, with after spell
    /// </summary>
    public void InitializeMe(Brain brain, System.Action onAfterSpell)
    {
        _onAfterSpell = onAfterSpell;
        InitializeMe(brain);        
    }
    /// <summary>
    /// Damage is from code, with after spell
    /// </summary>
    public void InitializeMe(Brain brain, PassDataContainer passData, System.Action onAfterSpell)
    {
        pd = passData;
        _onAfterSpell = onAfterSpell;
        InitializeMe(brain); 
    }
    #endregion

    #region EVENTS, CALLBACKS
    public System.Action onEnd;
    public System.Action<Brain> onHitTarget;
    System.Action _onAfterSpell;
    void CallEv_OnEnd()
    {
        if (!spellActive) return;
        _onAfterSpell?.Invoke();
        spellActive = false;
        visual.StopDefault();
        StartCoroutine(endDelay());
    
        IEnumerator endDelay()
        {
            yield return extraLifeTimeForParticles ? Ga.me.wait30 : Ga.me.wait00;
            Destroy(gameObject);
        }
    }
    #endregion

    void Update()
    {
        if (!spellActive) return;
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

    public void HitGeneric<T>(T targetGeneric, out Brain targetsBrain) where T : Component
    {
        Brain oustedTargetsBrain = null;
        foreach (PassData item in pd.data)
        {
            switch (item)
            {
                case PassDataDamage dam:
                case PassDataManaShield manaShield:
                case PassDataEffect effect:
                    if (targetHasBrain(out Brain brHealth))
                    {
                        PassDataDamage d = item as  PassDataDamage;
                        if (d != null && d.addSpellVelocity) d.spellsVelocity = myRigid.linearVelocity;
                        brHealth.health.HealthInjectData(pd);
                    }
                    break;
                
                case PassDataStats stats:
                    if (targetHasBrain(out Brain brCharacter))
                    {
                        brCharacter.character.BuffInjectData(stats);
                    }
                    break;
                
                case PassDataKnockBack knockBack:
                case PassDataDash dash:
                    if (targetHasBrain(out Brain brLoco))
                    {
                        brLoco.loco.LocoInjectData(pd);
                    }
                    break;

                case PassDataSpell spellData:
                    if (targetGeneric.TryGetComponent(out SpellMain targetSpell) &&
                        Utils.CanTargetFaction(OwnersBrain.Faction, targetSpell.OwnersBrain.Faction, myFactionTarget))
                    {
                        for (int i = 0; i < spellData.pair.Length(); i++)
                        {
                            if (targetSpell.id != spellData.pair.GetValue(i).id) continue;

                            switch (spellData.pair.GetKey(i))
                            {
                                case PassData.HitEffectOnSpell.Nullify:
                                    targetSpell.MyPhase = Phase.EndStart;
                                    break;
                                case PassData.HitEffectOnSpell.Reflect:
                                    Vector3 newDirection = Utils.Direction(myTransform.position,
                                        targetSpell.myTransform.position);
                                    targetSpell.transporter.ReflectProjectile(OwnersBrain, newDirection);
                                    break;
                            }
                        }
                        oustedTargetsBrain = targetSpell.OwnersBrain;
                    }
                    break;
            }
        }
        targetsBrain = oustedTargetsBrain;
        return;
        
        bool targetHasBrain(out Brain br)
        {
            if (targetGeneric.TryGetComponent(out br) &&
                Utils.CanTargetFaction(OwnersBrain.Faction, br.Faction, myFactionTarget))
            {
                oustedTargetsBrain = br;
                return true;
            }
            return false;
        }
    }

    #region TRIGGERS/COLLSIONS
    
    bool UseTriggers() => spellActive && Random.value < hitChance;
    void OnTriggerEnter(Collider other)
    {
        if (UseTriggers()) onTrigEnter.Invoke(other);
    }
    void OnTriggerExit(Collider other)
    {
        if (UseTriggers()) onTrigExit.Invoke(other);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (UseTriggers()) onCollEnter.Invoke(collision);
    }
    #endregion

}
