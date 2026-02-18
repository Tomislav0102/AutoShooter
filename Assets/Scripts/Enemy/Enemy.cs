using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Enemy : EventBus
{
    public Transform myTarget;
    [SerializeField] EnMovement startingMovement;
    [ShowInInspector][ReadOnly] EnMovement _movement;
    public NavMeshAgent agent;
    float _timerIdle;
    const float CONST_IdleMaxTime = 2f;
    Quaternion _idleTargetRot;
    bool _idleIsTurning;
    [SerializeField] Transform[] waypoints;
    int _counterWaypoints;
    const float CONST_FleeDistance = 10f;
    Transform _followTarget;
    const float CONST_FollowDistance = 2f;

    public bool isPlayerSummon;
    Vector3 GetRandomPosition(Transform surface)
    {
        float width = surface.localScale.x * 0.5f;
        float length = surface.localScale.y * 0.5f;
        float x = surface.position.x + Random.Range(-width, width);
        float z = surface.position.z + Random.Range(-length, length);
        return new Vector3(x, 0f, z);
    }
    
    void Start()
    {
        _movement = startingMovement;
        isPlayerSummon = Utils.IsInLayerMask(gameObject, gm.layPlayer);
        if (isPlayerSummon) _followTarget = gm.playerTransform;
    }

    void Update()
    {
        switch (_movement)
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

    public void AllTargetsGone()
    {
        agent.ResetPath();
        _movement = EnMovement.Stationary;
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
            Vector3 newDestination = GetRandomPosition(gm.ground);
            print($"New roam destination {newDestination}");
            agent.destination = newDestination;
        }
    }
    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            _movement = EnMovement.Stationary;
            return;
        }
        
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.destination = waypoints[_counterWaypoints].position;
            print($"New roam destination {waypoints[_counterWaypoints].position}");
            _counterWaypoints = (1 + _counterWaypoints) % waypoints.Length;
        }
    }
    public void Follow()
    {
        if (_followTarget == null)
        {
            _movement = EnMovement.Stationary;
            return;
        }
        if (Vector3.Distance(transform.position, _followTarget.position) > CONST_FollowDistance)
        {
            agent.destination = _followTarget.position;
        }
    }

    void Chase()
    {
        if (Vector3.Distance(transform.position, myTarget.position) > agent.stoppingDistance)
        {
            agent.destination = myTarget.position;
        }
    }
    void Flee()
    {
        if (Vector3.Distance(transform.position, myTarget.position) < CONST_FleeDistance)
        {
            Vector3 direction = transform.position - myTarget.position;
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

