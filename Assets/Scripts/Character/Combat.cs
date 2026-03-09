using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combat : EventBus, IInit
{
    protected Brain br;
    public bool IsReady { get; set; } //only called in children (because they're on scene)
    public bool isAttacking;
    public virtual Transform MyTarget { get; set; }
    [SerializeField] protected float damage;
    public int rangeMelee, rangeRanged;
    [SerializeField] protected float detectRange = float.MaxValue;
    float _searchWait;
    HashSet<Transform> _targets;

    

    
    public virtual void Initialize(Brain brain)
    {
        br = brain;
        _searchWait = Random.Range(0f, 0.2f) + 0.5f;
        _targets = Utils.IsInLayerMask(gameObject, gm.layPlayer) ? gm.allEnemies : gm.playersTeam;
        StartCoroutine(SearchTargetCoroutine());
    }
    
    public virtual void AE_Attack(int num = 0){}
    
    IEnumerator SearchTargetCoroutine()
    {
        while (true)
        {
            CheckTarget();
            yield return new WaitForSeconds(_searchWait);
        }
    }

    protected override void CallEv_OnCharDeath(Transform tr)
    {
        base.CallEv_OnCharDeath(tr);
        CheckTarget();
    }

    void CheckTarget()
    {
        // if (enemy.MyTarget != null) return;
        MyTarget = Utils.ClosestTransform(br.loco.myTransform.position, _targets, detectRange);
    }

}
