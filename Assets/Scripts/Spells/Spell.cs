using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Spell : EventBus
{
     Brain _ownersBrain;
     public CompSpell comp;
    [ReadOnly] public Faction myFaction = Faction.Neutral;
    [SerializeField] protected List<Faction> factionsToTarget;
    [SerializeField] Element element;
    [SerializeField] protected float damageMod = 1f;
    [SerializeField] protected Stats offenseSkill = Stats.MagicDamage;
    public bool canBeBlocked;
    public float radius;
    [SerializeField] protected float speed;
    [Range(0, 20)][SerializeField] protected int knockBack;
    [SerializeField] protected float lifeTime;
    [SerializeField] GameObject afterEffect;
    public Transform anchor;
    protected DamageData dam;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    float _timerLife;
    bool _endDelayStarted;

    public virtual void InitializeMe(Brain brain)
    {
        _ownersBrain = brain;
        myFaction = _ownersBrain.faction;
        comp.myCollider.enabled = false;
        comp.myRigid.isKinematic = true;
        comp.myCollider.radius = radius;
        if (comp.myVisualization != null) comp.myVisualization.localScale = radius * 2 * Vector3.one;
        dam = new DamageData()
        {
            attacker = _ownersBrain.myTransform,
            damage = damageMod * _ownersBrain.myChar.GetStat(offenseSkill),
            canBeBlocked = canBeBlocked,
            knockBack = knockBack,
            element = element,
        };
        Physics.IgnoreCollision(comp.myCollider, _ownersBrain.myCollider);
    }

    public virtual void InitializeMe(Brain brain, Stats offense)
    {
        offenseSkill = offense;
        InitializeMe(brain);
    }

    protected virtual void Update()
    {
        if (anchor != null) comp.myTransform.position = anchor.position;

        _timerLife += Time.deltaTime;
        if (_timerLife >= lifeTime && !_endDelayStarted) StartCoroutine(Delay());
    }
    IEnumerator Delay()
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
            Spell spell = Instantiate(afterEffect, comp.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform).GetComponent<Spell>();
            spell.InitializeMe(_ownersBrain);
        }
    }
    public virtual void OnEnd()
    {

        Destroy(gameObject);
    }
}
