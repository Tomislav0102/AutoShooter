using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class E_Loco : EventBus, ILocomotion
{
    public enum EnMovement { Stationary, Roam, Patrol, Follow, Chase, Flee }
    public enum MultiShot { AllAtOnce, Consecutive, Random }

    Brain _brain;
    public void TargetRelay(bool isNull)
    {
        if (isNull)
        {
            moveCurrent = moveIdling;
            agent.stoppingDistance = _startingStoppingDistance;
        }
        else
        {
            moveCurrent = moveFighting;
            agent.stoppingDistance = _combat.attackRange * 0.8f;
        }
        moveCurrent = isNull ? moveIdling : moveFighting;
    }
    EnemyCombat _combat;
    [SerializeField] EnMovement moveIdling;
    [SerializeField] EnMovement moveFighting;
    [ReadOnly] public EnMovement moveCurrent;
    public NavMeshAgent agent;
    Coroutine _pushCoroutine;
    float _startingStoppingDistance;
    float _timerIdle;
    const float CONST_IdleMaxTime = 2f;
    Quaternion _idleTargetRot;
    bool _idleIsTurning;
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


    public void Initialize(Brain brain)
    {
        _brain = brain;
        isPlayerSummon = Utils.IsInLayerMask(gameObject, gm.layPlayer);
        _combat = GetComponent<EnemyCombat>();
        _startingStoppingDistance = agent.stoppingDistance;
        moveCurrent = moveIdling;
        if (isPlayerSummon)
        {
            _followTarget = gm.playerTransform;
            gm.playersTeam.Add(_brain.MyTransform);
        }
        else
        {
            gm.allEnemies.Add(_brain.MyTransform);
        }
        agent.enabled = true;
    }

    public bool IsReady { get; set; }

    public IEnumerator Dash()
    {
        yield break;
    }

    void Update()
    {
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
    }

    public void PushMe(Vector3 origin, float intensity = 1f)
    {
        if (_pushCoroutine != null) StopCoroutine(_pushCoroutine);
        _pushCoroutine = StartCoroutine(PushMeSequence());
            
        IEnumerator PushMeSequence()
        {
            agent.enabled = false;
            _brain.myRigid.isKinematic = false;
            _brain.myRigid.collisionDetectionMode = CollisionDetectionMode.Continuous;
            float duration = 0.2f;
            float pushPower = 80 * intensity;
            float velocityModifier = 1f;
            Vector3 dir = _brain.MyTransform.position - origin;
            dir.y = 0;
            dir.Normalize();
            while (velocityModifier > 0f)
            {
                velocityModifier -= Time.deltaTime / duration;
                _brain.myRigid.linearVelocity = velocityModifier * pushPower * dir;
                yield return null;
            }
            _brain.myRigid.linearVelocity = Vector3.zero;
            _brain.myRigid.isKinematic = true;
            _brain.myRigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
            agent.enabled = true;
        }
    }


    #region NAVIGATION
    void Idle()
    {
        if (_idleIsTurning)
        {
            _brain.MyTransform.rotation = Quaternion.Slerp(_brain.MyTransform.rotation, _idleTargetRot, Time.deltaTime * 3f);
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
        if (Utils.Distance(_brain.MyTransform.position, _followTarget.position) > agent.stoppingDistance)
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
            print("chase");
        if (_brain.MyTarget == null) return;
            print("chase1");
        if (Utils.Distance(_brain.MyTransform.position, _brain.MyTarget.position) > agent.stoppingDistance)
        {
            agent.destination = _brain.MyTarget.position;
        }
    }
    void Flee()
    {
        if (_brain.MyTarget == null) return;
        if (Utils.Distance(_brain.MyTransform.position, _brain.MyTarget.position) < CONST_FleeDistance)
        {
            Vector3 direction = _brain.MyTransform.position - _brain.MyTarget.position;
            direction.y = 0f;
            direction.Normalize();
            Vector3 targetPosition = _brain.MyTransform.position + 2f * direction;
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

