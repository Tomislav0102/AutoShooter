using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyCombat : EventBus, ICombat
{
    public bool IsReady { get; set; }
    protected Brain br;
    protected E_Loco myLoco;
    [SerializeField] protected float damage;
    [SerializeField] float rof;
    [SerializeField] public float attackRange;
    [SerializeField] protected float detectRange = float.MaxValue;
    [SerializeField] bool faceTarget = true;
    float _timerAttack;
    float _searchWait;
    HashSet<Transform> _targets;
    
    [SerializeField] E_Projectile projectilePrefab;
    [SerializeField] float projectileSpeed;

    public virtual void Initialize(Brain brain)
    {
        br = brain;
        myLoco = br.GetComponent<E_Loco>();
        _searchWait = Random.Range(0f, 0.2f) + 0.5f;
        _targets = Utils.IsInLayerMask(gameObject, gm.layPlayer) ? gm.allEnemies : gm.playersTeam;
        StartCoroutine(SearchTargetCoroutine());
        IsReady = true;
    }

    public Transform MyTarget
    {
        get => _myTarget;
        set
        {
            _myTarget = value;
            myLoco.TargetRelay(!value);
        }
    }
    [ShowInInspector][ReadOnly] Transform _myTarget;

    [field: SerializeField] public bool IsAttacking { get; set; }
    public virtual void AE_Attack(int num = 0) { }

    void Update()
    {
        if (MyTarget == null) return;
        if (faceTarget) transform.LookAt(new Vector3(MyTarget.position.x, transform.position.y, MyTarget.position.z));
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
        MyTarget = Utils.ClosestTransform(transform.position, _targets, detectRange);
    }

    protected void SpawnProjectile(Transform spawnPointTransform)
    {
        E_Projectile projectile = Instantiate(projectilePrefab, spawnPointTransform.position, spawnPointTransform.rotation);
        ProjectilePassData passData = new ProjectilePassData((string st) =>
        {
            print(st);
        }, br, damage, projectileSpeed);
        projectile.InitializeMe(passData);
    }
    protected void SpawnProjectile(Vector3 pos, Quaternion rot)
    {
        E_Projectile projectile = Instantiate(projectilePrefab, pos, rot);
        ProjectilePassData passData = new ProjectilePassData((string st) =>
        {
            print(st);
        }, br, damage, projectileSpeed);
        projectile.InitializeMe(passData);
    }

}
