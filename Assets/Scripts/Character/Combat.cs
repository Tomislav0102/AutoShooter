using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Combat : SerializedMonoBehaviour, IInit
{
    public virtual Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _searchWait = Random.Range(0f, 0.2f) + 0.5f;
            _targets = value.faction == Faction.Ally ? Ga.me.team[Faction.Foe] : Ga.me.team[Faction.Ally];
            _targetAim = transform.GetChild(0);
            StartCoroutine(SearchTargetCoroutine());
        }
    }
    Brain _br;
    public bool IsReady { get; set; } //only called in children (because they're on scene)
    public bool isAttacking;
    [field: SerializeField] public virtual Transform MyTarget { get; set; }
    Transform _targetAim;
    protected float distance;
    [SerializeField] protected float detectRange = float.MaxValue;
    float _searchWait;
    HashSet<Transform> _targets;
    
    
    protected virtual void Update()
    {
        if (MyTarget == null)
        {
            distance = -1;
            _targetAim.localPosition = Vector3.zero;
        }
        else
        {
            distance = Utils.Distance(Br.loco.myTransform.position, MyTarget.position);
            _targetAim.position = MyTarget.position;
        }
    }

    public virtual void FromAnimEv_Attack(int num = 0) { }

    IEnumerator SearchTargetCoroutine()
    {
        yield return new WaitForSeconds(_searchWait * 2);
        while (true)
        {
            CheckTarget();
            yield return new WaitForSeconds(_searchWait);
        }
    }

    void CheckTarget()
    { 
      //  if (MyTarget != null) return;
        MyTarget = Utils.ClosestTransform(Br.loco.myTransform.position, _targets, detectRange);
    }

}
