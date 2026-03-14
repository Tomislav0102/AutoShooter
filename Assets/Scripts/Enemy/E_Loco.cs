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
    public enum RangeArea { Melee, Ranged, OutOfRange }
    [ReadOnly] public RangeArea ra = RangeArea.OutOfRange;
    public enum Movement { Stationary, Roam, Patrol, Follow, Chase, Flee }
    public enum MultiShot { AllAtOnce, Consecutive, Random }
    public Movement moveIdlingDefault;
    public Movement moveFightingDefault;
    [ReadOnly] public Movement moveCurrent;
    public NavMeshAgent agent;
    float _timerIdle;
    const float CONST_IdleMaxTime = 2f;
    Quaternion _idleTargetRot = Quaternion.identity;
    bool _idleIsTurning;
    int _counterWaypoints;
    const float CONST_FleeDistance = 10f;
    Transform _followTarget;
    const float CONST_FollowDistance = 5f;

    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            moveCurrent = moveIdlingDefault;
            if (value.faction == Faction.Ally)
            {
                _followTarget = Ga.me.playerTransform;
            }
            agent.enabled = true;
            agent.speed = moveSpeed;
            ra = RangeArea.OutOfRange;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!agent.enabled) return;
        switch (moveCurrent)
        {
            case Movement.Stationary:
                Idle();
                break;
            case Movement.Roam:
                Roam();
                break;
            case Movement.Patrol:
                Patrol();
                break;
            case Movement.Follow:
                Follow();
                break;
            case Movement.Chase:
                Chase();
                break;
            case Movement.Flee:
                Flee();
                break;
        }
        
        agent.speed = 0f;
        bool att = false;
        bool att1 = false;
        bool move = false;
        switch (ra)
        {
            case RangeArea.Melee:
                att = true;
                LookAtMethod();
                break;
            case RangeArea.Ranged:
                att1 = true;
                LookAtMethod();
                break;
            case RangeArea.OutOfRange:
                agent.speed = moveSpeed;
                move = true;
                break;
        }
        Toggle_Move(move);
        AttInputEnemy(att);
        Att1InputEnemy(att1);

    }


    #region NAVIGATION
    void Idle()
    {
        if (_idleIsTurning)
        {
            Br.loco.myTransform.rotation = Quaternion.Slerp(Br.loco.myTransform.rotation, _idleTargetRot, Time.deltaTime * 3f);
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
            Vector3 newDestination = GetRandomPosition(Ga.me.spawnArea);
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
        if (Ga.me.waypoints == null || Ga.me.waypoints.Length == 0)
        {
            moveCurrent = Movement.Stationary;
            return;
        }
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.destination = Ga.me.waypoints[_counterWaypoints].position;
            _counterWaypoints = (1 + _counterWaypoints) % Ga.me.waypoints.Length;
        }
    }

    void Follow()
    {
        if (_followTarget == null)
        {
            moveCurrent = Movement.Stationary;
            return;
        }
        agent.stoppingDistance = CONST_FollowDistance;
        if (Utils.Distance(Br.loco.myTransform.position, _followTarget.position) > agent.stoppingDistance)
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
        if (Br.combat.MyTarget == null) return;
        if (Utils.Distance(Br.loco.myTransform.position, Br.combat.MyTarget.position) > agent.stoppingDistance)
        {
            agent.destination = Br.combat.MyTarget.position;
        }
    }

    void Flee()
    {
        if (Br.combat.MyTarget == null) return;
        if (Utils.Distance(Br.loco.myTransform.position, Br.combat.MyTarget.position) < CONST_FleeDistance)
        {
            Vector3 direction = Br.loco.myTransform.position - Br.combat.MyTarget.position;
            direction.y = 0f;
            direction.Normalize();
            Vector3 targetPosition = Br.loco.myTransform.position + 2f * direction;
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

