using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class E_Loco : Loco
{
    public enum EnMovement { Stationary, Roam, Patrol, Follow, Chase, Flee }
    public enum MultiShot { AllAtOnce, Consecutive, Random }
    [SerializeField] EnMovement moveIdling;
    [SerializeField] EnMovement moveFighting;
    [ReadOnly] public EnMovement moveCurrent;
    public NavMeshAgent agent;
    float _startingStoppingDistance;
    float _timerIdle;
    const float CONST_IdleMaxTime = 2f;
    Quaternion _idleTargetRot = Quaternion.identity;
    bool _idleIsTurning;
    int _counterWaypoints;
    const float CONST_FleeDistance = 10f;
    Transform _followTarget;
    const float CONST_FollowDistance = 5f;

    [ReadOnly] public bool isPlayerSummon;

    public override void Initialize(Brain brain)
    {
        base.Initialize(brain);
        isPlayerSummon = Utils.IsInLayerMask(gameObject, gm.layPlayer);
        _startingStoppingDistance = agent.stoppingDistance;
        moveCurrent = moveIdling;
        if (isPlayerSummon)
        {
            _followTarget = gm.playerTransform;
            gm.playersTeam.Add(br.loco.myTransform);
        }
        else
        {
            gm.allEnemies.Add(br.loco.myTransform);
        }
        agent.enabled = true;
        agent.speed = moveSpeed;
    }


    protected override void Update()
    {
        base.Update();
        if (!agent.enabled) return;
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
        MoveInputEnemy(agent.velocity != Vector3.zero);
    }

    public void TargetRelay(float attRange)
    {
        if (attRange < 0)
        {
            moveCurrent = moveIdling;
            agent.stoppingDistance = _startingStoppingDistance;
        }
        else
        {
            moveCurrent = moveFighting;
            agent.stoppingDistance = attRange * 0.9f;
        }
    }



    #region NAVIGATION
    void Idle()
    {
        if (_idleIsTurning)
        {
            br.loco.myTransform.rotation = Quaternion.Slerp(br.loco.myTransform.rotation, _idleTargetRot, Time.deltaTime * 3f);
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

        Vector3 GetRandomPosition(Transform surface)
        {
            float width = surface.localScale.x * 0.5f;
            float length = surface.localScale.z * 0.5f;
            float x = surface.position.x + Random.Range(-width, width);
            float z = surface.position.z + Random.Range(-length, length);
            return new Vector3(x, 0f, z);
        }
    }

    void Patrol()
    {
        if (gm.waypoints == null || gm.waypoints.Length == 0)
        {
            moveCurrent = EnMovement.Stationary;
            return;
        }
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.destination = gm.waypoints[_counterWaypoints].position;
            _counterWaypoints = (1 + _counterWaypoints) % gm.waypoints.Length;
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
        if (Utils.Distance(br.loco.myTransform.position, _followTarget.position) > agent.stoppingDistance)
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
        if (br.combat.MyTarget == null) return;
        if (Utils.Distance(br.loco.myTransform.position, br.combat.MyTarget.position) > agent.stoppingDistance)
        {
            agent.destination = br.combat.MyTarget.position;
        }
    }

    void Flee()
    {
        if (br.combat.MyTarget == null) return;
        if (Utils.Distance(br.loco.myTransform.position, br.combat.MyTarget.position) < CONST_FleeDistance)
        {
            Vector3 direction = br.loco.myTransform.position - br.combat.MyTarget.position;
            direction.y = 0f;
            direction.Normalize();
            Vector3 targetPosition = br.loco.myTransform.position + 2f * direction;
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
        Gizmos.DrawSphere(agent.destination, 0.2f);
    }

}

