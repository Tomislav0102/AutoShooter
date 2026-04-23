using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Combat : MonoBehaviour, IInit
{
    public virtual Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _searchWait = Random.Range(0f, 0.2f) + 0.5f;
            _targets = value.faction == Faction.GoodGuys ? Ga.me.team[Faction.BadGuys] : Ga.me.team[Faction.GoodGuys];
            targetControl = new TargetControl(value);
            StartCoroutine(SearchTargetCoroutine());
            
            IEnumerator SearchTargetCoroutine()
            {
                yield return new WaitForSeconds(_searchWait * 2);
                while (true)
                {
                    // MyTarget = Utils.ClosestTransform(Br.myTransform.position, _targets, detectRange);
                    targetControl.AssignTarget(Utils.ClosestTransform(Br.myTransform.position, _targets, detectRange));
                    yield return new WaitForSeconds(_searchWait);
                }
            }

        }
    }

    Brain _br;
    public bool IsInitialized { get; set; } //only called in children (because they're on scene)
    public TargetControl targetControl;

    public virtual Transform MyTarget
    {
        get => _myTarget;
        set
        {
            _myTarget = value;
            distanceToTarget = Utils.Distance(Br.myTransform.position, MyTarget.position);
        }
    }

    Transform _myTarget;
    protected float distanceToTarget;
    [SerializeField] protected float detectRange = float.MaxValue;
    float _searchWait;
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

public class TargetControl
{
    public Brain Br { get; set; }

    Transform _target;
    bool _hasTarget;
    float _distance;

    public TargetControl(Brain brain)
    {
        Br = brain;
    }

    public void AssignTarget(Transform target)
    {
        _target = target;
        _hasTarget = _target != null;
    }

    public bool HasTarget(out float distance)
    {
        distance = 0f;
        if (_hasTarget)
        {
            distance = Utils.Distance(Br.myTransform.position, _target.position);
            return true;
        }
        return false;
    }
    public bool HasTarget(out Vector3 pos)
    {
        pos = Vector3.zero;
        if (_hasTarget)
        {
            pos = _target.position;
            return true;
        }
        return false;
    }

}
