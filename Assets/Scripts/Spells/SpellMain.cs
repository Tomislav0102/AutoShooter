using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.Universal;


public class SpellMain : MonoBehaviour
{
    bool HasId() => !string.IsNullOrEmpty(idSpell);
    [Button] [HideIf(nameof(HasId))] [GUIColor("red")]
    void SetId() => idSpell = System.Guid.NewGuid().ToString();
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
    public enum CounteringSpellEffect { Nullify, Reflect }

    #endregion
    public bool debug;
    [FoldoutGroup("Refs")] public Transform myTransform;
    [FoldoutGroup("Refs")] public Rigidbody myRigid;
    [FoldoutGroup("Refs")] public SphereCollider mySphereCollider;
    [FoldoutGroup("Refs")] public CapsuleCollider myCapsuleCollider;
    [FoldoutGroup("Refs")] [SerializeField] DecalProjector warning;
    [FoldoutGroup("Refs")] [SerializeField] GameObject effectGo;
    [FoldoutGroup("Refs")] public SpellTransporter transporter;
    [FoldoutGroup("Refs")] public SpellVisual visual;

    public ColliderType colliderType;
    public bool isInterrupt;
    [SerializeField] bool canBeBlocked;
    [SerializeField] bool canBeDodged;
    [SerializeField] bool triggersOnHitEvent;
    [SerializeField] bool ignoreCollisionsWithOwner = true;
    [SerializeField] bool removeBuffsAfterEnd;
    [SerializeField] bool extraLifeTimeForParticles = true;
    HashSet<Brain> _targetsBuff = new HashSet<Brain>();
    [ReadOnly] public bool spellActive;
    [ReadOnly] public string idSpell;
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
                    if (ignoreCollisionsWithOwner) Physics.IgnoreCollision(mySphereCollider, OwnersBrain.myCollider);
                    mySphereCollider.enabled = true;
                    break;
                case ColliderType.Capsule:
                    if (ignoreCollisionsWithOwner) Physics.IgnoreCollision(myCapsuleCollider, OwnersBrain.myCollider);
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
                    if (_timerPhase < warningDelay) warning.enabled = true;
                    break;
                case Phase.SpellRuns:
                    _timerPhase = 0f;
                    warning.enabled = false;
                    visual.PlayDefault();
                    break;
                case Phase.EndStart:
                    MyPhase = terminateOnHit ? Phase.EndEnd : Phase.SpellRuns;
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
    [InfoBox("Lifetime info: -0 Endless | 0 Instant | +0 Specific")]
    public float lifeTime;
    bool LifeTimeIs0() => lifeTime == 0f;
    [Tooltip("if false it will play for 'lifetime' seconds. Does nothing if 'lifetime' == 0.")]
    [SerializeField, HideIf(nameof(LifeTimeIs0))] bool terminateOnHit = true;
    public HashSet<Collider> collidersDetected = new HashSet<Collider>();
    public Specialty specialty;
    [HideInInspector] public int reflexCount = 2;
    [SerializeField, FoldoutGroup("Events")] UnityEvent<Collider> onTrigEnter;
    [SerializeField, FoldoutGroup("Events")] UnityEvent<Collider> onTrigExit;
    [SerializeField, FoldoutGroup("Events")] UnityEvent<Collision> onCollEnter;


    #region INITIALIZATION
    /// <summary>
    /// Damage is from inspector, no after spell
    /// </summary>
    public void InitializeMe(Brain brain)
    {
        if (brain == null) //it's a hack, but it works
        {
            print("Destroying spell, no brain");
            Destroy(gameObject);
            return;
        }
      //  if (guid.Equals(System.Guid.Empty)) print($"{gameObject.name} ID is 0, need to assign ID in inspector!");
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
        warning.size = new Vector3(areaOfEffect, areaOfEffect, 20);
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
    public System.Action<Brain> onHitTarget;
    System.Action _onAfterSpell;
    void CallEv_OnEnd()
    {
        if (!spellActive) return;
        _onAfterSpell?.Invoke();
        spellActive = false;
        foreach (Brain item in _targetsBuff)
        {
            if (pd.hasStats)
            {
                for (int i = 0; i < pd.stats.Length; i++)
                {
                    item.character.CharacterInjectData(GenChange.Remove, pd.stats[i]);
                }
            }
            if (pd.hasEffect)
            {
                for (int i = 0; i < pd.effects.Length; i++)
                {
                    item.status.StatusInjectData(GenChange.Remove, pd.effects[i]);
                }
                    
            }
        }
        visual.StopDefault();
        Destroy(gameObject, extraLifeTimeForParticles ? 5f : 0f);
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

    
    public void HitGeneric<T>(T targetGeneric, out Brain targetsBrain, bool callOnHitEvent = true) where T : Component
    {
        if (!(targetGeneric.TryGetComponent(out targetsBrain) &&
              Utils.CanTargetFaction(OwnersBrain.Faction, targetsBrain.Faction, myFactionTarget))) return;
        if (callOnHitEvent) onHitTarget?.Invoke(targetsBrain);
        if (hasReflected(targetsBrain)) return;
        if (targetsBrain.health.IsImmuneToSpell(idSpell)) return;

        if (pd.hasManaShield)  targetsBrain.health.HealthInjectDataManaShield(pd.manaShieldPoints);
        if (pd.hasDash) targetsBrain.loco.LocoInjectDataDash(pd.dashPower, pd.dashDirection);

        if (pd.hasEffect)
        {
            for (int i = 0; i < pd.effects.Length; i++)
            {
                targetsBrain.status.StatusInjectData(GenChange.Add, pd.effects[i]);
                if (removeBuffsAfterEnd) _targetsBuff.Add(targetsBrain);
            }
        }
        if (pd.hasStats)
        {
            for (int i = 0; i < pd.stats.Length; i++)
            {
                targetsBrain.character.CharacterInjectData(GenChange.Add, pd.stats[i]);
                if (removeBuffsAfterEnd) _targetsBuff.Add(targetsBrain);
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
            targetsBrain.health.HealthInjectDataDamage(pd, canBeBlocked, canBeDodged, specialty);
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


