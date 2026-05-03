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
            _targets = value.faction == Faction.GoodGuys ? Ga.me.team[Faction.BadGuys] : Ga.me.team[Faction.GoodGuys];
            StartCoroutine(SearchTargetCoroutine(Random.Range(0.1f, 0.2f)));
            
            IEnumerator SearchTargetCoroutine(float searchWait)
            {
                yield return new WaitForSeconds(searchWait * 2);
                while (true)
                {
                    MyTarget = Utils.ClosestTransform(value.myTransform.position, _targets, detectRange);
                    yield return new WaitForSeconds(searchWait);
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

    //cache
    protected Dictionary<Element, float> damMelee = new Dictionary<Element, float>();
    protected Dictionary<Element, float> damRanged = new Dictionary<Element, float>();
    protected Dictionary<Element, float> damUltimate = new Dictionary<Element, float>();
    protected int counterHits;
    protected int counterHitReceived;


    public virtual void FromAnimEv_Attack(int num = 0)
    {
        counterHits++;
    }

    public virtual void FromAnimEv_Ultimate(int num = 0)
    {
    }

}