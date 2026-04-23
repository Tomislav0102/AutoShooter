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
    public enum Movement { Stationary, Roam, Patrol, Follow, Chase, Flee, Frozen }
    public enum MultiShot { AllAtOnce, Consecutive, Random }
    public Movement moveIdlingDefault;
    public Movement moveFightingDefault;
    [ReadOnly] public Movement moveCurrent;
    public NavMeshAgent agent;
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            moveCurrent = moveIdlingDefault;
            agent.enabled = true;
            agent.speed = moveSpeed;
            ra = RangeArea.OutOfRange;
            IsInitialized = true;
        }
    }
    bool _canMoveNavigation;
    bool _canMoveCombat;


    #region MOVEMENT SPECIFIC VARIABLES
    float _timerGeneral;
    float _timerStationary, _timerStationaryMaxTime;
    float StationaryTimeIdle() => Random.Range(5f, 10f);
    float StationaryTimeRotating() => Random.Range(1f, 3f);
    bool _stationaryIsTurning; 
    Vector3 _stationaryRotAxis;
    int _counterWaypoints;
    const float CONST_FleeDistance = 10f;
    Transform FollowTarget()
    {
        if (_followTarget == null)  _followTarget = Ga.me.playerTransform;
        return _followTarget;
    }
    Transform _followTarget;
    const float CONST_FollowDistance = 5f;
    float _chaseRange;
    #endregion


    void Update()
    {
        if (!agent.enabled) return;
        _canMoveNavigation = true;
        lookAtTarget = true;
        switch (moveCurrent)
        {
            case Movement.Stationary:
                Stationary();
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
            case Movement.Frozen:
                Frozen();
                break;
        }

        _canMoveCombat = false;
        agent.speed = 0f;
        bool att = false;
        bool att1 = false;
        switch (ra)
        {
            case RangeArea.Melee:
                att = true;
                if (lookAtTarget) LookAtMethod();
                break;
            case RangeArea.Ranged:
                att1 = true;
                if (lookAtTarget) LookAtMethod();
                break;
            case RangeArea.OutOfRange:
                agent.speed = moveSpeed;
                _canMoveCombat = true;
                break;
        }
        AttInputEnemy(att);
        Att1InputEnemy(att1);
        
        bool canMove = _canMoveNavigation && _canMoveCombat;
        Toggle_Move(canMove);
        agent.speed = canMove ? moveSpeed : 0f;
    }


    protected override void ControlsEnabled(bool isEnabled)
    {
        base.ControlsEnabled(isEnabled);
        agent.enabled = isEnabled;
        Br.myRigid.isKinematic = isEnabled;
        Br.myRigid.collisionDetectionMode = isEnabled ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
    }

    #region NAVIGATION
    void Stationary() //no movement, just rotation
    {
        _canMoveNavigation = false;
        if (_stationaryIsTurning)
        {
            Br.myTransform.Rotate(_stationaryRotAxis, _timerStationary * 0.2f);
        }
        
        _timerStationary += Time.deltaTime;
        if (_timerStationary >= _timerStationaryMaxTime)
        {
            _timerStationary = 0f;
            _stationaryRotAxis = Random.value > 0.5f ? Vector3.up : Vector3.down;
            _timerStationaryMaxTime = _stationaryIsTurning ? StationaryTimeIdle() : StationaryTimeRotating();
            _stationaryIsTurning = !_stationaryIsTurning;
        }
    }
    void Roam()
    {
        if (agent.remainingDistance <= 0.5f) MethodRoam();
        _timerGeneral += Time.deltaTime;
        if (_timerGeneral > 10f) MethodRoam();
        
        void MethodRoam()
        {
            Vector3 newDestination = Utils.GetRandomPosition(Ga.me.LevelMan.spawnArea);
            // print($"New roam destination {newDestination}");
            agent.destination = newDestination;
            _timerGeneral = 0f;
        }
    }
    void Patrol()
    {
        if (Ga.me.waypoints == null || Ga.me.waypoints.Length == 0)
        {
            moveCurrent = Movement.Stationary;
            return;
        }
        
        if (agent.remainingDistance <= 0.5f) MethodPatrol();
        _timerGeneral += Time.deltaTime;
        if (_timerGeneral > 10f) MethodPatrol();

        void MethodPatrol()
        {
            agent.destination = Ga.me.waypoints[_counterWaypoints].position;
            _counterWaypoints = (1 + _counterWaypoints) % Ga.me.waypoints.Length;
            _timerGeneral = 0f;
        }
    }
    void Follow()
    {
        if (Utils.Distance(Br.myTransform.position, FollowTarget().position) > CONST_FollowDistance)
        {
            agent.destination = FollowTarget().position;
        }
        else
        {
            if (agent.hasPath) agent.ResetPath();
            Stationary();
        }
    }
    void Chase()
    {
        if (Br.combat.MyTarget == null) return;
        if (ra == RangeArea.OutOfRange)
        {
            agent.destination = Br.combat.MyTarget.position;
        }
        else
        {
            if (agent.hasPath) agent.ResetPath();
        }
    }
    void Flee()
    {
        if (Br.combat.MyTarget == null) return;
        if (Utils.Distance(Br.myTransform.position, Br.combat.MyTarget.position) < CONST_FleeDistance)
        {
            Vector3 direction = Br.myTransform.position - Br.combat.MyTarget.position;
            direction.y = 0f;
            direction.Normalize();
            Vector3 targetPosition = Br.myTransform.position + 2f * direction;
            NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas);
            if (!hit.hit) return;
            agent.destination = hit.position;
        }
    }

    void Frozen()//no movement or rotation
    {
        lookAtTarget = false;
    } 
    #endregion

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(agent.destination, 0.2f);
    }

}


// public class E_Loco : Loco
// {
//     public enum RangeArea { Melee, Ranged, OutOfRange }
//     [ReadOnly] public RangeArea ra = RangeArea.OutOfRange;
//     public enum Movement { Stationary, Roam, Patrol, Follow, Chase, Flee, Frozen }
//     public enum MultiShot { AllAtOnce, Consecutive, Random }
//     public Movement moveIdlingDefault;
//     public Movement moveFightingDefault;
//     [ReadOnly] public Movement moveCurrent;
//     public NavMeshAgent agent;
//     public override Brain Br
//     {
//         get => base.Br;
//         set
//         {
//             base.Br = value;
//             moveCurrent = moveIdlingDefault;
//             agent.enabled = true;
//             agent.speed = moveSpeed;
//             ra = RangeArea.OutOfRange;
//             IsInitialized = true;
//         }
//     }
//     bool _canMoveNavigation;
//     bool _canMoveCombat;
//
//
//     #region MOVEMENT SPECIFIC VARIABLES
//     float _timerGeneral;
//     float _timerStationary, _timerStationaryMaxTime;
//     float StationaryTimeIdle() => Random.Range(5f, 10f);
//     float StationaryTimeRotating() => Random.Range(1f, 3f);
//     bool _stationaryIsTurning; 
//     Vector3 _stationaryRotAxis;
//     int _counterWaypoints;
//     const float CONST_FleeDistance = 10f;
//     Transform FollowTarget()
//     {
//         if (_followTarget == null)  _followTarget = Ga.me.playerTransform;
//         return _followTarget;
//     }
//     Transform _followTarget;
//     const float CONST_FollowDistance = 5f;
//     float _chaseRange;
//     #endregion
//
//     public override Disposition Disp //not used
//     {
//         get => base.Disp;
//         set
//         {
//             base.Disp = value;
//             switch (value)
//             {
//                 case Disposition.Relaxed:
//                     break;
//                 case Disposition.Wary:
//                     break;
//                 case Disposition.Fighting:
//                     break;
//             }
//
//         }
//     }
//
//     void Update()
//     {
//         if (!agent.enabled) return;
//         _canMoveNavigation = true;
//         lookAtTarget = true;
//         switch (moveCurrent)
//         {
//             case Movement.Stationary:
//                 Stationary();
//                 break;
//             case Movement.Roam:
//                 Roam();
//                 break;
//             case Movement.Patrol:
//                 Patrol();
//                 break;
//             case Movement.Follow:
//                 Follow();
//                 break;
//             case Movement.Chase:
//                 Chase();
//                 break;
//             case Movement.Flee:
//                 Flee();
//                 break;
//             case Movement.Frozen:
//                 Frozen();
//                 break;
//         }
//
//         _canMoveCombat = false;
//         agent.speed = 0f;
//         bool att = false;
//         bool att1 = false;
//         switch (ra)
//         {
//             case RangeArea.Melee:
//                 att = true;
//                 if (lookAtTarget) LookAtMethod();
//                 break;
//             case RangeArea.Ranged:
//                 att1 = true;
//                 if (lookAtTarget) LookAtMethod();
//                 break;
//             case RangeArea.OutOfRange:
//                 agent.speed = moveSpeed;
//                 _canMoveCombat = true;
//                 break;
//         }
//         AttInputEnemy(att);
//         Att1InputEnemy(att1);
//         
//         bool canMove = _canMoveNavigation && _canMoveCombat;
//         Toggle_Move(canMove);
//         agent.speed = canMove ? moveSpeed : 0f;
//     }
//
//
//     protected override void ControlsEnabled(bool isEnabled)
//     {
//         base.ControlsEnabled(isEnabled);
//         agent.enabled = isEnabled;
//         Br.myRigid.isKinematic = isEnabled;
//         Br.myRigid.collisionDetectionMode = isEnabled ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
//     }
//
//     #region NAVIGATION
//     void Stationary() //no movement, just rotation
//     {
//         _canMoveNavigation = false;
//         if (_stationaryIsTurning)
//         {
//             Br.myTransform.Rotate(_stationaryRotAxis, _timerStationary * 0.2f);
//         }
//         
//         _timerStationary += Time.deltaTime;
//         if (_timerStationary >= _timerStationaryMaxTime)
//         {
//             _timerStationary = 0f;
//             _stationaryRotAxis = Random.value > 0.5f ? Vector3.up : Vector3.down;
//             _timerStationaryMaxTime = _stationaryIsTurning ? StationaryTimeIdle() : StationaryTimeRotating();
//             _stationaryIsTurning = !_stationaryIsTurning;
//         }
//     }
//     void Roam()
//     {
//         if (agent.remainingDistance <= 0.5f) MethodRoam();
//         _timerGeneral += Time.deltaTime;
//         if (_timerGeneral > 10f) MethodRoam();
//         
//         void MethodRoam()
//         {
//             Vector3 newDestination = Utils.GetRandomPosition(Ga.me.LevelMan.spawnArea);
//             // print($"New roam destination {newDestination}");
//             agent.destination = newDestination;
//             _timerGeneral = 0f;
//         }
//     }
//     void Patrol()
//     {
//         if (Ga.me.waypoints == null || Ga.me.waypoints.Length == 0)
//         {
//             moveCurrent = Movement.Stationary;
//             return;
//         }
//         
//         if (agent.remainingDistance <= 0.5f) MethodPatrol();
//         _timerGeneral += Time.deltaTime;
//         if (_timerGeneral > 10f) MethodPatrol();
//
//         void MethodPatrol()
//         {
//             agent.destination = Ga.me.waypoints[_counterWaypoints].position;
//             _counterWaypoints = (1 + _counterWaypoints) % Ga.me.waypoints.Length;
//             _timerGeneral = 0f;
//         }
//     }
//     void Follow()
//     {
//         if (Utils.Distance(Br.myTransform.position, FollowTarget().position) > CONST_FollowDistance)
//         {
//             agent.destination = FollowTarget().position;
//         }
//         else
//         {
//             if (agent.hasPath) agent.ResetPath();
//             Stationary();
//         }
//     }
//     void Chase()
//     {
//         if (Br.combat.MyTarget == null) return;
//         if (ra == RangeArea.OutOfRange)
//         {
//             agent.destination = Br.combat.MyTarget.position;
//         }
//         else
//         {
//             if (agent.hasPath) agent.ResetPath();
//         }
//     }
//     void Flee()
//     {
//         if (Br.combat.MyTarget == null) return;
//         if (Utils.Distance(Br.myTransform.position, Br.combat.MyTarget.position) < CONST_FleeDistance)
//         {
//             Vector3 direction = Br.myTransform.position - Br.combat.MyTarget.position;
//             direction.y = 0f;
//             direction.Normalize();
//             Vector3 targetPosition = Br.myTransform.position + 2f * direction;
//             NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas);
//             if (!hit.hit) return;
//             agent.destination = hit.position;
//         }
//     }
//
//     void Frozen()//no movement or rotation
//     {
//         lookAtTarget = false;
//     } 
//     #endregion
//
//     void OnDrawGizmos()
//     {
//         if (!Application.isPlaying) return;
//         Gizmos.color = Color.red;
//         Gizmos.DrawSphere(agent.destination, 0.2f);
//     }
//
// }
