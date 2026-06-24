using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class E_Loco : Loco
{
    public enum RangeArea { Melee, Ranged, OutOfRange }
    [ReadOnly] public RangeArea weaponRange = RangeArea.OutOfRange;
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
            weaponRange = RangeArea.OutOfRange;
            Renderer myRenderer =  GetComponentInChildren<Renderer>();
            myRenderer.material = myMaterials[(int)value.Faction];
            IsInitialized = true;
        }
    }
    [SerializeField] Material[] myMaterials;
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
        if (_followTarget == null)  _followTarget = Ga.me.team.playerTransform;
        return _followTarget;
    }
    Transform _followTarget;
    const float CONST_FollowDistance = 5f;
    float _chaseRange;
    #endregion

    
    void Update()
    {
        if (!controlsEnabled) return;
        
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
        switch (weaponRange)
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
    protected override IEnumerator PushMeSequence(Vector3 dir, float deltaIntensity = 1)
    {
        yield return base.PushMeSequence(dir, deltaIntensity);
        controlsEnabled = false;
        float effIntensity = 5 * deltaIntensity;
        effIntensity = Mathf.Clamp(effIntensity, 0f, 30f);
        Vector3 velocity = effIntensity * dir;
        agent.ResetPath();
        while (velocity.magnitude > 0.2f)
        {
            Vector3 translationThisFrame = velocity * Time.deltaTime;
            agent.Move(translationThisFrame);
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, effIntensity * 1.5f * Time.deltaTime);
            yield return null;
        }
        controlsEnabled = true;
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
        _timerGeneral += Time.deltaTime;
        if (agent.remainingDistance <= 0.5f || _timerGeneral > 10f)
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

        _timerGeneral += Time.deltaTime;
        if (agent.remainingDistance <= 0.5f || _timerGeneral > 10f)
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
        
        if (weaponRange == RangeArea.OutOfRange && !anim.GetBool("isAttacking"))
        {
            agent.destination = Br.combat.MyTarget.position;
        }
        else if (agent.hasPath) agent.ResetPath();
    }
    void Flee()
    {
        if (Br.combat.MyTarget == null) return;
        
        if (Br.combat.distanceToTarget < CONST_FleeDistance)
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

