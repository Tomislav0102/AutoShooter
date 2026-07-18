using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Combat : EventBus, IInit
{
    [System.Serializable]
    class WeaponSet
    {
        public SpellMain spellMain;
        public AnimAttackType attackType; 
        public float range; 
    }
    [SerializeField] WeaponSet[] weapons;

    protected SpellMain GetSpellByAttackType(AnimAttackType animAttackType)
    {
        foreach (WeaponSet item in weapons)
        {
            if (item.attackType == animAttackType) return item.spellMain;
        }
        return null;
    }
    public virtual Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            StartCoroutine(SearchTargetCoroutine(Random.Range(0.1f, 0.2f)));
            if (weapons.Length > 0)
            {
                Array.Sort(weapons, (x, y) => x.range.CompareTo(y.range));
            }
            return;

            IEnumerator SearchTargetCoroutine(float delay)
            {
                yield return new WaitForSeconds(delay);
                while (true)
                {
                    List<Transform> foundTargets = Utils.ChooseGroupTransforms(value.myTransform.position, Ga.me.team.ValidTargets(value.Faction));
                    MyTarget = foundTargets.Count == 0 ? null : foundTargets[0];
                    yield return new WaitForSeconds(0.15f);
                }
            }
        }
    }

    Brain _br;

    [field:SerializeField, ReadOnly] public bool IsInitialized { get; set; }

    public virtual Transform MyTarget
    {
        get => _myTarget;
        set
        {
            _myTarget = value;
            if (value != null)
            {
                distanceToTarget = Utils.Distance(Br.myTransform.position, value.position);
            }
            else
            {
                Br.loco.AttackAnimation(null);
            }
        }
    }
    [ShowInInspector, ReadOnly] Transform _myTarget;
    protected float distanceToTarget;
    [ReadOnly] public AnimAttackType? InAttackRange()
    {
        if (MyTarget == null) return null;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (distanceToTarget <= weapons[i].range) return weapons[i].attackType;
        }
        return null;
    }
    
    float _timerBlockReady;
    const int CONST_BlockTimer = 2;

    //cache
    protected Dictionary<Element, float> damMelee = new Dictionary<Element, float>();
    protected Dictionary<Element, float> damRanged = new Dictionary<Element, float>();
    protected Dictionary<Element, float> damUltimate = new Dictionary<Element, float>();
    protected PassDataDamage[] pdDamageMelee;
    protected PassDataDamage[] pdDamageRanged;
    protected PassDataDamage[] pdDamageUltimate;

    public virtual void CombatEventRegistered(CombatEvent combatEvent, Brain otherBrain = null)
    {
        switch (combatEvent)
        {
            case CombatEvent.Strike:
                break;
            case CombatEvent.Hit:
            case CombatEvent.Miss:
                CombatEventRegistered(CombatEvent.Strike);
                break;
            case CombatEvent.GetHit:
                break;
            case CombatEvent.Block:
                break;
            case CombatEvent.Kill:
                break;
        }   
    }

    public void CheckBlock(out bool blocked, Brain otherBrain = null)
    {
        blocked = _timerBlockReady >= 0f && Random.value * 100 < Br.character.GetStat(Stats.Block);
        if (!blocked) return;
        StartCoroutine(resetBlockTimer());
        CombatEventRegistered(CombatEvent.Block, otherBrain);
        Br.loco.Block();
        return;
        
        IEnumerator resetBlockTimer()
        {
            _timerBlockReady = CONST_BlockTimer;
            while (_timerBlockReady > 0)
            {
                _timerBlockReady -= Time.deltaTime;
                yield return null;
            }
            _timerBlockReady = 0;
        }

    }

    public void CheckDodge(out bool dodged)
    {
        dodged = false;
    }
    public virtual void FromAnimEv_Attack(int num = 0)
    {
        
    }

    public virtual void FromAnimEv_Ultimate(int num = 0)
    {
    }

}