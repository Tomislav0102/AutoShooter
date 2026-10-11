using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyLoco : MonoBehaviour, IIniBrain
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            behCurrent = behIdlingDefault;
            value.agent.enabled = true;
            value.agent.angularSpeed = Ga.me.gameData.agentRotSpeed;

        }
    }
    Brain _br;

    public enum Behavior { Stationary, Roam, Patrol, Follow, Chase, Flee }
    public Behavior behIdlingDefault;
    public Behavior behFightingDefault;
    [ReadOnly] public Behavior behCurrent;
    [SerializeField] EnemyCombat eCombat;
    #region MOVEMENT SPECIFIC VARIABLES
    float _timerGeneral;
    float _timerStationary, _timerStationaryMaxTime;
    float StationaryTimeIdle() => Random.Range(5f, 10f);
    float StationaryTimeRotating() => Random.Range(1f, 3f);
    bool _stationaryIsTurning; 
    Vector3 _stationaryRotAxis;
    int _counterWaypoints;
    Transform FollowTarget() //placeholder
    {
        if (_followTarget == null)  _followTarget = Ga.me.team.playersBrain.myTransform;
        return _followTarget;
    }
    Transform _followTarget;
    const float CONST_FollowDistance = 5f;
    #endregion

    
    void Update()
    {
        Br.loco.OvrOrientation = false;
        AnimAttackType? animAttackType = eCombat.InAttackRange();
        switch (behCurrent)
        {
            case Behavior.Stationary:
                stationary();
                break;

            case Behavior.Roam:
                roam();
                void roam()
                {
                    if (Br.loco.OvrMotion) return;
                    _timerGeneral += Time.deltaTime;
                    if (Br.agent.remainingDistance <= 0.5f || _timerGeneral > 10f)
                    {
                        Vector3 newDestination = Utils.GetRandomPosition(Ga.me.LevelMan.spawnArea);
                        Br.agent.destination = newDestination;
                        _timerGeneral = 0f;
                    }
                }
                break;

            case Behavior.Patrol:
                patrol();
                void patrol()
                {
                    if (Br.loco.OvrMotion) return;
                    if (Ga.me.waypoints == null || Ga.me.waypoints.Length == 0)
                    {
                        behCurrent = Behavior.Stationary;
                        return;
                    }

                    _timerGeneral += Time.deltaTime;
                    if (Br.agent.remainingDistance <= 0.5f || _timerGeneral > 10f)
                    {
                        Br.agent.destination = Ga.me.waypoints[_counterWaypoints].position;
                        _counterWaypoints = (1 + _counterWaypoints) % Ga.me.waypoints.Length;
                        _timerGeneral = 0f;
                    }
                }
                break;

            case Behavior.Follow:
                follow();
                void follow()
                {
                    if (Br.loco.OvrMotion) return;
                    if (Utils.Distance(Br.myTransform.position, FollowTarget().position) > CONST_FollowDistance)  Br.agent.destination = FollowTarget().position;
                    else
                    {
                        if (Br.agent.hasPath) Br.agent.ResetPath();
                        stationary();
                    }
                }
                break;

            case Behavior.Chase:
                chase();
                void chase()
                {
                    if (Br.loco.OvrMotion) return;
                    Br.loco.OvrOrientation = true;
                    if (animAttackType == null)
                    {
                        Br.agent.destination = Br.combat.MyTarget.myTransform.position;
                    }
                    else if (Br.agent.hasPath) Br.agent.ResetPath();
                }
                break;

            case Behavior.Flee:
                flee();
                void flee()
                {
                    if (Br.loco.OvrMotion) return;
                    if (animAttackType == null) return;
                    Vector3 direction = Br.myTransform.position - Br.combat.MyTarget.myTransform.position;
                    direction.y = 0f;
                    direction.Normalize();
                    Vector3 targetPosition = Br.myTransform.position + 2f * direction;
                    NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas);
                    if (!hit.hit) return;
                    Br.agent.destination = hit.position;
                }
                break;
        }

        Br.loco.AttackAnimation(animAttackType);
        bool canMove = behCurrent != Behavior.Stationary && animAttackType == null && !Br.loco.IsAttackAnimationPlaying();
        Br.loco.Toggle_Move(canMove);
        if (!Br.loco.OvrMotion) Br.agent.speed = canMove ? Br.loco.moveSpeed : 0;
        if (Br.loco.OvrOrientation) Br.loco.Orientation(Br.combat.MyTarget.myTransform);
        Br.agent.updateRotation = !Br.loco.OvrOrientation;

        void stationary()
        {
           // if (!Br.loco.OvrOrientation) return; //maybe will cause bugs
            if (_stationaryIsTurning) Br.myTransform.Rotate(_stationaryRotAxis, _timerStationary * 0.2f);

            _timerStationary += Time.deltaTime;
            if (_timerStationary < _timerStationaryMaxTime) return;
            
            _timerStationary = 0f;
            _stationaryRotAxis = Random.value > 0.5f ? Vector3.up : Vector3.down;
            _timerStationaryMaxTime = _stationaryIsTurning ? StationaryTimeIdle() : StationaryTimeRotating();
            _stationaryIsTurning = !_stationaryIsTurning;
        }
    }


    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Br.agent.destination, 0.2f);
    }

}

