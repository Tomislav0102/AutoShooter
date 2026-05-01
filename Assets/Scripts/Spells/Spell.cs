using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class MyTimer
{
    float _maxTime;
    float _timer;
    bool _isLooping;
    public bool completed;
    System.Action _onComplete;

    public MyTimer(float maxTime, System.Action onComplete, bool isLooping = false)
    {
        _maxTime = maxTime;
        _onComplete = onComplete;
        _isLooping = isLooping;
    }

    public void UpdateLoop()
    {
        if (!_isLooping && completed) return;
        _timer += Time.deltaTime;
        if (_timer > _maxTime)
        {
            _timer = 0;
            _onComplete?.Invoke();
            completed = true;
        }
    }
}

[System.Serializable]
public class SpellParticles
{
    public enum ParticleSizeChange
    {
        Emission_Shape, 
        TransformScale, //ps needs to have empty parent that will be scaled. Ps.transform is never scaled by code, only in inspector (e.g. fireball)
        Other
    }
    [SerializeField] ParticleSystem ps;
    public ParticleSizeChange particleSizeChange;

    public void InitializeMe(float areaOfEffect)
    {
        if (ps == null) return;
        switch (particleSizeChange)
        {
            case  ParticleSizeChange.Emission_Shape:
                var emission = ps.emission;
                emission.rateOverTime = areaOfEffect * 5;
                var shape = ps.shape;
                shape.radius = areaOfEffect * 0.5f;
                break;
            case  ParticleSizeChange.TransformScale:
                ps.transform.parent.localScale = areaOfEffect * Vector3.one;
                break;
        }
        ps.Play();
    }
}

public class Spell :SerializedMonoBehaviour
{
    Brain _ownersBrain;
    public CompSpell comp;
    [ReadOnly] public Faction myFaction;
    [SerializeField] protected FactionToTarget myFactionTarget = FactionToTarget.Enemy;
    public float areaOfEffect = 1;
    [SerializeField] float startDelay;
    [SerializeField] protected float speed;
    [SerializeField] protected float lifeTime;
    [SerializeField] Spell afterEffect;
    public Transform anchor;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    float _timerLife;
    bool _endDelayStarted;
    protected MyTimer timerStartDelay;
    [SerializeField, BoxGroup("Particles", false)] protected SpellParticles spellParticles;
    
    [SerializeField, BoxGroup] protected InjectHealth injectHealthData;
    [SerializeField, BoxGroup] protected bool useInspectorDamageData;
    [SerializeField, ShowIf(nameof(useInspectorDamageData)), BoxGroup] protected Dictionary<Element, float> inspectorDamage =  new Dictionary<Element, float>();


    void InitializeMeShared(Brain brain)
    {
        _ownersBrain = brain;
        myFaction = _ownersBrain.faction;
        if (useInspectorDamageData || injectHealthData.damage == null) injectHealthData.damage = inspectorDamage;
        
        comp.myRigid.isKinematic = true;
        comp.visualization.localScale = areaOfEffect * Vector3.one;
        comp.warningRend.transform.localScale = areaOfEffect * Vector3.one;
        if (comp.mySphereCollider != null)
        {
            Physics.IgnoreCollision(comp.mySphereCollider, _ownersBrain.myCollider);
            comp.mySphereCollider.enabled = false;
            comp.mySphereCollider.radius = areaOfEffect * 0.5f;
        }

        if (comp.myCapsuleCollider != null) //not used
        {
            Physics.IgnoreCollision(comp.myCapsuleCollider, _ownersBrain.myCollider);
            comp.myCapsuleCollider.enabled = false;
            comp.myCapsuleCollider.height = areaOfEffect;
            comp.myCapsuleCollider.center = areaOfEffect * 0.5f * Vector3.forward;
        }
        
        if (startDelay > 0) comp.warningRend.enabled = true;
        timerStartDelay = new MyTimer(startDelay, () =>
        {
            comp.warningRend.enabled = false;
        });
    }


    public virtual void InitializeMe(Brain brain, Dictionary<Element, float> damage = null)
    {
        injectHealthData.damage = damage;
        InitializeMeShared(brain);
    }


    protected virtual void Update()
    {
        // if (startDelay > 0)
        // {
        //     startDelay -= Time.deltaTime;
        //     comp.warningRend.enabled = true;
        //     return;
        // }
        // comp.warningRend.enabled = false;
        timerStartDelay.UpdateLoop();
        if (!timerStartDelay.completed) return;
        if (anchor != null) comp.myTransform.position = anchor.position;

        _timerLife += Time.deltaTime;
        if (!_endDelayStarted && _timerLife >= lifeTime) StartCoroutine(DelayEnd());
    }

    IEnumerator DelayEnd()
    {
        _endDelayStarted = true;
        yield return new WaitForFixedUpdate();
        OnEnd();
    }

    protected void SetSpeed()
    {
        comp.myRigid.linearVelocity = speed * comp.myTransform.forward;
    }

    protected void AfterEffect()
    {
        if (afterEffect != null)
        {
            Spell spell = Instantiate(afterEffect, comp.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            spell.InitializeMeShared(_ownersBrain);
        }
    }

    public void OnEnd()
    {
        Destroy(gameObject);
    }

}
