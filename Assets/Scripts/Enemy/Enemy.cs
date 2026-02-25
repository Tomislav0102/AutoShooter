using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Enemy : EventBus, ICharacter
{
    public Transform MyTarget
    {
        get => _myTarget;
        set
        {
            _myTarget = value;
            if (value == null)
            {
                moveCurrent = moveIdling;
                agent.stoppingDistance = _startingStoppingDistance;
            }
            else
            {
                moveCurrent = moveFighting;
                agent.stoppingDistance = combat.attackRange * 0.8f;
            }
            moveCurrent = value == null ? moveIdling : moveFighting;
        }
    }
    [ShowInInspector][ReadOnly] Transform _myTarget;
    [field: SerializeField] public Transform MyTransform { get; set; }
    [SerializeField] EnemyCombat combat;
    [SerializeField] Health health;
    [SerializeField] EnMovement moveIdling;
    [SerializeField] EnMovement moveFighting;
    [ReadOnly] public EnMovement moveCurrent;
    public NavMeshAgent agent;
    float _startingStoppingDistance;
    float _timerIdle;
    const float CONST_IdleMaxTime = 2f;
    Quaternion _idleTargetRot;
    bool _idleIsTurning;
    [SerializeField] Transform[] waypoints;
    int _counterWaypoints;
    const float CONST_FleeDistance = 10f;
    Transform _followTarget;
    const float CONST_FollowDistance = 5f;

    [ReadOnly] public bool isPlayerSummon;
    Vector3 GetRandomPosition(Transform surface)
    {
        float width = surface.localScale.x * 0.5f;
        float length = surface.localScale.z * 0.5f;
        float x = surface.position.x + Random.Range(-width, width);
        float z = surface.position.z + Random.Range(-length, length);
        return new Vector3(x, 0f, z);
    }

    protected override void Awake()
    {
        base.Awake();
        isPlayerSummon = Utils.IsInLayerMask(gameObject, gm.layPlayer);
        
    }

    void Start()
    {
        _startingStoppingDistance = agent.stoppingDistance;
        moveCurrent = moveIdling;
        if (isPlayerSummon)
        {
            _followTarget = gm.playerTransform;
            gm.playersTeam.Add(transform);
        }
        else
        {
            gm.allEnemies.Add(transform);
        }
        combat.InitializeMe(this);
        health.InitializeMe(this);
    }

    void Update()
    {
        switch (moveCurrent)
        {
            case EnMovement.Stationary:
                Idle();
                break;
            case EnMovement.Roam:
                Roam();
                break;
            case EnMovement.Patrol:
                Patrol();
                break;
            case EnMovement.Follow:
                Follow();
                break;
            case EnMovement.Chase:
                Chase();
                break;
            case EnMovement.Flee:
                Flee();
                break;
        }
    }


    #region NAVIGATION
    void Idle()
    {
        if (_idleIsTurning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _idleTargetRot, Time.deltaTime * 3f);
        }
        else
        {
            _idleTargetRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }
        
        _timerIdle += Time.deltaTime;
        if (_timerIdle >= CONST_IdleMaxTime)
        {
            _timerIdle = 0f;
            _idleIsTurning = !_idleIsTurning;
        }
    }
    void Roam()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 newDestination = GetRandomPosition(GameManager.Instance.spawnArea);
            // print($"New roam destination {newDestination}");
            agent.destination = newDestination;
        }

    }
    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            moveCurrent = EnMovement.Stationary;
            return;
        }
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.destination = waypoints[_counterWaypoints].position;
            _counterWaypoints = (1 + _counterWaypoints) % waypoints.Length;
        }
    }
    void Follow()
    {
        if (_followTarget == null)
        {
            moveCurrent = EnMovement.Stationary;
            return;
        }
        agent.stoppingDistance = CONST_FollowDistance;
        if (Vector3.Distance(transform.position, _followTarget.position) > agent.stoppingDistance)
        {
            agent.destination = _followTarget.position;
        }
        else
        {
            Idle();
        }
    }

    void Chase()
    {
        if (MyTarget == null) return;
        if (Vector3.Distance(transform.position, MyTarget.position) > agent.stoppingDistance)
        {
            agent.destination = MyTarget.position;
        }
    }
    void Flee()
    {
        if (MyTarget == null) return;
        if (Vector3.Distance(transform.position, MyTarget.position) < CONST_FleeDistance)
        {
            Vector3 direction = transform.position - MyTarget.position;
            direction.y = 0f;
            direction.Normalize();
            Vector3 targetPosition = transform.position + 2f * direction;
            NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas);
            if (!hit.hit) return;
            agent.destination = hit.position;
        }
    }

    #endregion

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(agent.destination, 0.1f);
    }

}

