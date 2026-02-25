using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyCombat : EventBus
{
    protected Enemy enemy;
    [SerializeField] protected float damage;
    [SerializeField] float rof;
    [SerializeField] public float attackRange;
    [SerializeField] protected float detectRange = float.MaxValue;
    [SerializeField] bool faceTarget = true;
    float _timerAttack;
    float _searchWait;
    HashSet<Transform> _targets;


    public virtual void InitializeMe(Enemy en)
    {
        enemy = en;
        _searchWait = Random.Range(0f, 0.2f) + 0.5f;
        _targets = enemy.isPlayerSummon ? gm.allEnemies : gm.playersTeam;
        StartCoroutine(SearchTargetCoroutine());
    }
    
    void Update()
    {
        if (enemy.MyTarget == null) return;
        if (faceTarget) transform.LookAt(new Vector3(enemy.MyTarget.position.x, transform.position.y, enemy.MyTarget.position.z));
        _timerAttack += Time.deltaTime;
        if (_timerAttack >= rof)
        {
            _timerAttack = 0f;
            Attack();
        }
    }

    IEnumerator SearchTargetCoroutine()
    {
        while (true)
        {
            CheckTarget();
            yield return new WaitForSeconds(_searchWait);
        }
    }

    protected virtual void Attack()
    {
        
    }

    protected override void CallEv_OnCharDeath(Transform tr)
    {
        base.CallEv_OnCharDeath(tr);
        CheckTarget();
    }

    void CheckTarget()
    {
       // if (enemy.MyTarget != null) return;
        enemy.MyTarget = Utils.ClosestTransform(transform.position, _targets, detectRange);
    }
}
