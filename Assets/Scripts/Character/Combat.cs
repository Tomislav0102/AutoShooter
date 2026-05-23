using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Combat : EventBus, IInit
{
    public virtual Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _targets = value.Faction == Faction.GoodGuys ? Ga.me.team[Faction.BadGuys] : Ga.me.team[Faction.GoodGuys];
            StartCoroutine(SearchTargetCoroutine(Random.Range(0.1f, 0.2f)));
            return;
            
            IEnumerator SearchTargetCoroutine(float delay)
            {
                yield return new WaitForSeconds(delay);
                while (true)
                {
                    MyTarget = Utils.ClosestTransform(value.myTransform.position, _targets, detectRange);
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
            if (value != null) distanceToTarget = Utils.Distance(Br.myTransform.position, value.position);
        }
    }
    [ShowInInspector, ReadOnly] Transform _myTarget;
    [ReadOnly] public float distanceToTarget;
    [SerializeField] protected float detectRange = float.MaxValue;
    HashSet<Transform> _targets;
    
    float _timerBlockReady;
    const int CONST_BlockTimer = 2;


    //cache
    protected Dictionary<Element, float> damMelee = new Dictionary<Element, float>();
    protected Dictionary<Element, float> damRanged = new Dictionary<Element, float>();
    protected Dictionary<Element, float> damUltimate = new Dictionary<Element, float>();


    public void CheckBlock(out bool blocked)
    {
        blocked = _timerBlockReady >= 0 && Random.value * 100 < Br.myChar.GetStat(Stats.Block);
        if (!blocked) return;
        _timerBlockReady = CONST_BlockTimer;
        StartCoroutine(ResetBlockTimer());
        Br.myChar.ProcessSkillReqIncrease(SkillReq.Block);
        Br.loco.Block();
        return;
        
        IEnumerator ResetBlockTimer()
        {
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