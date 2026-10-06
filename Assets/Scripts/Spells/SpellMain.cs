using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class SpellMain : MonoBehaviour
{
    #region ENUMS
    public enum Phase
    {
        BeginWarning,
        SpellRuns,
        EndStart,
        EndEnd,
    }
    public enum Specialty
    {
        General,
        Melee,
        Projectile,
        NoReflection
    }
    public enum HitEffectOnSpell { Nullify, Reflect }

    #endregion
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
    [SerializeField] bool canBeBlocked;
    [SerializeField] bool canBeDodged;
    [SerializeField] bool triggersOnHitEvent;
    [ReadOnly] public bool spellActive;
    [ReadOnly] public PassData pd;
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
    public bool extraLifeTimeForParticles = true;
    [InfoBox("Lifetime info: -0 Endless | 0 Instant | +0 Specific")]
    public float lifeTime;
    bool LifeTimeIs0() => lifeTime == 0f;
    [Tooltip("if false it will play for 'lifetime' seconds. Does nothing if 'lifetime' == 0.")]
    [SerializeField, HideIf(nameof(LifeTimeIs0))] bool terminateOnHit = true;
    public HashSet<Collider> collidersDetected = new HashSet<Collider>();
    public Specialty specialty;
    [HideInInspector] public int reflexCount = 2;

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
        if (triggersOnHitEvent) onHitTarget += (Brain brainTargeted) =>
        {
            if (brainTargeted == null) OwnersBrain.combat.CombatEventRegistered(CombatEvent.Miss);
            else OwnersBrain.combat.CombatEventRegistered(CombatEvent.Hit, brainTargeted);
        };
        myRigid.isKinematic = true;
        if (isInterrupt) gameObject.layer = LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt);
        
        if (transporter.GetComponent<MeleeTransporter>() != null)  areaOfEffect = range;
        warningRend.transform.localScale = areaOfEffect * Vector3.one;
        mySphereCollider.radius = areaOfEffect * 0.5f;
        myCapsuleCollider.height = areaOfEffect;
        myCapsuleCollider.center = areaOfEffect * 0.5f * Vector3.forward;
        
        OvrPassData ovrPassData = GetComponent<OvrPassData>();
        if (ovrPassData != null) pd = ovrPassData.passData;
        if (pd == null)
        {
            print($"No PassData found, destroying {gameObject.name} spell.");
            Destroy(gameObject);
            return;
        }
        pd.myBrain = OwnersBrain;
        
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
    public void InitializeMe(Brain brain, PassData passData)
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
    public void InitializeMe(Brain brain, PassData passData, System.Action onAfterSpell)
    {
        pd = passData;
        _onAfterSpell = onAfterSpell;
        InitializeMe(brain); 
    }
    #endregion

    #region EVENTS, CALLBACKS
    public System.Action onEnd;
    public System.Action<Brain> onHitTarget { get; set; }
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
            yield return extraLifeTimeForParticles ? Ga.me.wait100 : Ga.me.wait00;
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
        if (!(targetGeneric.TryGetComponent(out targetsBrain) &&
              Utils.CanTargetFaction(OwnersBrain.Faction, targetsBrain.Faction, myFactionTarget))) return;
        if (hasReflected(targetsBrain)) return;
        if (targetsBrain.health.IsImmuneToSpell(id)) return;

        if (pd.hasManaShield)  targetsBrain.health.HealthInjectDataManaShield(pd.manaShieldPoints);
        if (pd.hasDash) targetsBrain.loco.LocoInjectDataDash(pd.dashPower, pd.dashDirection);

        if (pd.hasEffect)
        {
            for (int i = 0; i < pd.effects.Length; i++)
            {
                targetsBrain.status.StatusInjectData(GenChange.Add, pd.effects[i]);
            }
        }
        if (pd.hasStats)
        {
            for (int i = 0; i < pd.stats.Length; i++)
            {
                targetsBrain.character.CharacterInjectData(GenChange.Add, pd.stats[i]);
            }
        }
        if (pd.hasKnockback)
        {
            targetsBrain.loco.LocoInjectDataKnockback(pd.myBrain.myTransform.position, pd.knockbackPower, pd.knockbackDirection);
        }
        if (pd.hasDamage)
        {
            if (targetsBrain.status.HasEffect(Status.Effect.Invulnerable))
            {
                Ga.me.uiManager.FloatText(targetsBrain.myTransform.position, "Invulnerable!", Color.brown);
                return;
            }

            // if (pd.hasDamage && pd.spellsVelocity.Equals(Vector2.zero)) d.spellsVelocity = myRigid.linearVelocity;
            targetsBrain.health.HealthInjectDataDamage(pd, canBeBlocked, canBeDodged);
        }

        return;

        bool hasReflected(Brain brain)
        {
            if (reflexCount < 0) return false;

            int reflexStat;
            switch (specialty)
            {
                case Specialty.General:
                    if (Random.value < brain.character.GetStat(Stats.ReflexSpells) * 0.01f)
                    {
                        reflexCount--;
                        //need logic for this behaviour. Probably new spell instantiated (copy of reflected one) 
                        return true;
                    }
                    break;
                case Specialty.Melee:
                    reflexStat = brain.character.GetStat(Stats.ReflectMelee);
                    if (Random.value < reflexStat * 0.01f)
                    {
                        reflexCount--;
                        OwnersBrain = brain;
                        pd.DamageIncreasedByReflex(reflexStat);
                        HitGeneric(OwnersBrain, out _);
                        return true;
                    }
                    break;
                case Specialty.Projectile: 
                    reflexStat = brain.character.GetStat(Stats.ReflectProjectiles);
                    if (Random.value < reflexStat * 0.01f)
                    {
                        reflexCount--;
                        pd.DamageIncreasedByReflex(reflexStat);
                        transporter.ReflectProjectile(brain);
                        return true;
                    }
                    break;
            }
            return false;
        }

    }

    //used by Auras, Zones etc...
    public void HitExit(Collider other)
    {
        if (!(other.TryGetComponent(out Brain br) &&
              Utils.CanTargetFaction(OwnersBrain.Faction, br.Faction, myFactionTarget))) return;

        if (pd.hasStats)
        {
            PassData removeContainer = pd;
            for (int i = 0; i < removeContainer.stats.Length; i++)
            {
                br.character.CharacterInjectData(GenChange.Remove, removeContainer.stats[i]);
            }
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

    public static SpellMain Sp(SpellMain spell, Brain brain, bool brainsRotation = false)
    {
        return Instantiate(spell, brain.myTransform.position, brainsRotation ? brain.myTransform.rotation : Quaternion.identity, Ga.me.spells.myTransform);
    }
    public static SpellMain Sp(SpellMain spell, Vector3 pos)
    {
        return Instantiate(spell, pos, Quaternion.identity, Ga.me.spells.myTransform);
    }
    public static SpellMain Sp(SpellMain spell, Vector3 pos, Quaternion rot)
    {
        return Instantiate(spell, pos, rot, Ga.me.spells.myTransform);
    }
}


