using System;
using UnityEngine;

public class EnemyCombat : EventBus
{
    protected Enemy enemy;
    [SerializeField] protected float damage;
    [SerializeField] float rof;
    [SerializeField] protected float range;
    float _timerAttack;
    bool _isAttacking = true;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
        enemy.agent.stoppingDistance = range * 0.8f;
    }

    void Start()
    {
        CheckTarget();
    }

    void Update()
    {
        if (!_isAttacking) return;
        transform.LookAt(new Vector3(enemy.myTarget.position.x, transform.position.y, enemy.myTarget.position.z));
        _timerAttack += Time.deltaTime;
        if (_timerAttack >= rof)
        {
            _timerAttack = 0f;
            Attack();
        }
    }

    protected virtual void Attack()
    {
        
    }

    protected override void CallEv_OnAllyDeath(Transform tr)
    {
        base.CallEv_OnAllyDeath(tr);
        CheckTarget();
    }

    void CheckTarget()
    {
        enemy.myTarget = Utils.ClosestTransform(transform.position, gm.playersTeam);
        if (enemy.myTarget == null)
        {
            _isAttacking = false;
            enemy.AllTargetsGone();
        }

    }
}
