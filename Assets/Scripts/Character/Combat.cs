using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Combat : MonoBehaviour, IIniBrain, ITargetTracker
{
    [SerializeField] UnityEvent<Brain> brainEv;
    [SerializeField] UnityEvent<Brain> targetEv;
    [SerializeField] UnityEvent<int> animAttackEv;
    [SerializeField] UnityEvent<int> animAttackUltimateEv;
    [SerializeField] UnityEvent<CombatEvent, Brain, SpellMain.Specialty> combatRegisterEv;
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            brainEv.Invoke(value);
            StartCoroutine(searchTargetCoroutine(Random.Range(0.1f, 0.2f)));
            return;
            IEnumerator searchTargetCoroutine(float delay)
            {
                yield return new WaitForSeconds(delay);
                while (true)
                {
                    List<Brain> foundTargets = Utils.ChooseGroupTransforms(value.myTransform.position, Ga.me.team.ValidTargets(value.Faction));
                    MyTarget = foundTargets.Count == 0 ? null : foundTargets[0];
                    yield return Ga.me.wait02;
                }
            }
        }
    }
    Brain _br;

    public Brain MyTarget
    {
        get => _myTarget;
        set
        {
            _myTarget = value;
            if (_myTarget != null)
            {
                distanceToTarget = Utils.Distance(Br.myTransform.position, _myTarget.myTransform.position);
            }
            else
            {
                Br.loco.AttackAnimation(null);
            }
            targetEv.Invoke(_myTarget);
        }
    }
    [ShowInInspector, ReadOnly] Brain _myTarget;
    [HideInInspector] public float distanceToTarget;
    float _timerBlockReady;
    const int CONST_BlockTimer = 2;
    [HideInInspector] public int counterHit, counterMiss, counterStrike;

    
    public void CombatEventRegistered(CombatEvent combatEvent, Brain otherBrain = null, SpellMain.Specialty specialty = SpellMain.Specialty.General)
    {
        switch (combatEvent)
        {
            case CombatEvent.Hit:
                CombatEventRegistered(CombatEvent.Strike);
                counterHit++;
                break;
            case CombatEvent.Miss:
                CombatEventRegistered(CombatEvent.Strike);
                counterMiss++;
                break;
            case CombatEvent.Strike:
                counterStrike++;
                break;
        }   
        combatRegisterEv.Invoke(combatEvent, otherBrain, specialty);
    }

    public void CheckBlock(out bool blocked, Brain otherBrain)
    {
        blocked = _timerBlockReady >= 0f && Random.value * 100 < Br.character.GetStat(Stats.Block) && !IsFlanked(Br.myTransform, otherBrain.myTransform);
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

    public void CheckDodge(out bool dodged, Brain otherBrain)
    {
        CombatEventRegistered(CombatEvent.Dodge, otherBrain);
        dodged = false;
    }
    public void FromAnimEv_Attack(int num = 0)
    {
        animAttackEv.Invoke(num);
    }

    public void FromAnimEv_Ultimate(int num = 0)
    {
        animAttackUltimateEv.Invoke(num);
    }

    
    public static bool IsFlanked(Transform myTransform, Transform attackersTransform) //true -> target can be sneak attacked, false -> target can block
    {
        Vector2 attackDirection = (Utils.MakeV2(attackersTransform.position) - Utils.MakeV2(myTransform.position)).normalized;
        return Vector2.Dot(Utils.MakeV2(myTransform.forward), attackDirection) <= 0;
    }

}