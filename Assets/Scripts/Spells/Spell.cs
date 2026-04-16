using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class Spell : EventBus
{
     Brain _ownersBrain;
     public CompSpell comp;
    [ReadOnly] public Faction myFaction = Faction.Neutral;
    protected Faction TarFaction()
    {
        int val = (int)myFaction;
        print(val);
        val = (1 + val) % 2;
        print(val);
        return (Faction)val;
        if (myFaction == Faction.Player) return Faction.Monsters;
        if (myFaction == Faction.Monsters) return Faction.Player;
        return Faction.Neutral;
    }
    
    
    [SerializeField] Element element;
  //  protected float damage;
    public bool canBeBlocked;
    public float areaOfEffect = 1;
    [SerializeField] protected float speed;
    [Range(0, 20)][SerializeField] protected int knockBack;
    [SerializeField] protected float lifeTime;
    [SerializeField] GameObject afterEffect;
    public Transform anchor;
    protected DamageData damData;
    protected HashSet<Collider> collidersDetected = new HashSet<Collider>();
    float _timerLife;
    bool _endDelayStarted;

    
    public virtual void InitializeMe(Brain brain)
    {
        _ownersBrain = brain;
        myFaction = _ownersBrain.faction;
        if (myFaction == Faction.Player) myFaction = Faction.Monsters;
        else if (myFaction == Faction.Monsters) myFaction = Faction.Player;

        comp.myRigid.isKinematic = true;

        if (comp.mySphereCollider != null)
        {
            Physics.IgnoreCollision(comp.mySphereCollider, _ownersBrain.myCollider);
            comp.mySphereCollider.enabled = false;
            comp.mySphereCollider.radius = areaOfEffect * 0.5f;
        }
        if (comp.myCapsuleCollider != null)
        {
            Physics.IgnoreCollision(comp.myCapsuleCollider, _ownersBrain.myCollider);
            comp.myCapsuleCollider.enabled = false;
            comp.myCapsuleCollider.height = areaOfEffect;
            comp.myCapsuleCollider.center = areaOfEffect * 0.5f * Vector3.forward;
        }
        
        damData = new DamageData()
        {
            attacker = _ownersBrain.myTransform,
            damage = 1f, //debug
            canBeBlocked = canBeBlocked,
            knockBack = knockBack,
            element = element,
        };
    }

    public virtual void InitializeMe(Brain brain, float dam)
    {
        damData = new DamageData()
        {
            attacker = _ownersBrain.myTransform,
            damage = dam,
            canBeBlocked = canBeBlocked,
            knockBack = knockBack,
            element = element,
        };

        InitializeMe(brain);
    }

    public virtual void InitializeMe(Brain brain, float dam, HashSet<Collider> collidersToIgnore)
    {
        foreach (Collider col in collidersToIgnore) collidersDetected.Add(col);
        InitializeMe(brain, dam);
    }


    protected virtual void Update()
    {
        if (anchor != null) comp.myTransform.position = anchor.position;

        _timerLife += Time.deltaTime;
        if (!_endDelayStarted && _timerLife >= lifeTime) StartCoroutine(Delay());
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
    public void OnEnd()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || !Ga.me.spells.showDebug) return;
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(comp.myTransform.position, areaOfEffect);
    }
}
