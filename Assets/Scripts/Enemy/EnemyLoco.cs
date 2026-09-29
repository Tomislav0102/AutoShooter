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
            Renderer myRenderer =  GetComponentInChildren<Renderer>();
            myRenderer.material = myMaterials[(int)value.Faction];

        }
    }
    Brain _br;

    public enum Behavior { Stationary, Roam, Patrol, Follow, Chase, Flee }
    public Behavior behIdlingDefault;
    public Behavior behFightingDefault;
    [ReadOnly] public Behavior behCurrent;
    [SerializeField] Material[] myMaterials;
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
                        Br.agent.destination = Br.combat.MyTarget.position;
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
                    Vector3 direction = Br.myTransform.position - Br.combat.MyTarget.position;
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
        if (Br.loco.OvrOrientation) Br.loco.Orientation(Br.combat.MyTarget);
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


// public class E_Loco : Loco
// {
//     public enum RangeArea { Melee, Ranged, OutOfRange }
//     [ReadOnly] public RangeArea weaponRange = RangeArea.OutOfRange;
//
//     public enum Behavior { Stationary, Roam, Patrol, Follow, Chase, Flee }
//     [FormerlySerializedAs("moveIdlingDefault")] public Behavior behIdlingDefault;
//     [FormerlySerializedAs("moveFightingDefault")] public Behavior behFightingDefault;
//     public Behavior BehCurrent
//     {
//         get =>  _behCurrent;
//         set
//         {
//             _behCurrent = value;
//             switch (_behCurrent)
//             {
//                 case Behavior.Stationary:
//                     break;
//                 case Behavior.Roam:
//                     break;
//                 case Behavior.Patrol:
//                     break;
//                 case Behavior.Follow:
//                     break;
//                 case Behavior.Chase:
//                     break;
//                 case Behavior.Flee:
//                     break;
//             }
//         }
//     }
//     [ReadOnly, ShowInInspector] Behavior _behCurrent;
//     public override Brain Br
//     {
//         get => base.Br;
//         set
//         {
//             base.Br = value;
//             BehCurrent = behIdlingDefault;
//             loco.Br.agent.enabled = true;
//             loco.Br.agent.angularSpeed = Ga.me.gameData.agentRotSpeed;
//             weaponRange = RangeArea.OutOfRange;
//             Renderer myRenderer =  GetComponentInChildren<Renderer>();
//             myRenderer.material = myMaterials[(int)value.Faction];
//             IsInitialized = true;
//         }
//     }
//     [SerializeField] Material[] myMaterials;
//     bool _canMoveNavigation;
//     bool _canMoveCombat;
//     public override bool loco.OvrOrientation
//     {
//         get => base.loco.OvrOrientation;
//         set
//         {
//             base.loco.OvrOrientation = value;
//             loco.Br.agent.updateRotation = !value;
//         }
//     }
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
//     Transform FollowTarget() //placeholder
//     {
//         if (_followTarget == null)  _followTarget = Ga.me.team.playerTransform;
//         return _followTarget;
//     }
//     Transform _followTarget;
//     const float CONST_FollowDistance = 5f;
//     #endregion
//
//
//     bool IsAttackAnimationOver()
//     {
//         return !anim.GetCurrentAnimatorStateInfo(0 ).IsTag("Attacks");
//     }
//
//     void Update()
//     {
//         if (OvrMove) loco.OvrOrientation = true;
//         if (loco.OvrOrientation) Orientation(loco.Br.combat.MyTarget);
//         _canMoveNavigation = true;
//         _canMoveCombat = false;
//
//         switch (BehCurrent)
//         {
//             case Behavior.Stationary:
//                 stationary();
//                 break;
//
//             case Behavior.Roam:
//                 Roam();
//                 void Roam()
//                 {
//                     if (OvrMove) return;
//                     _timerGeneral += Time.deltaTime;
//                     if (loco.Br.agent.remainingDistance <= 0.5f || _timerGeneral > 10f)
//                     {
//                         Vector3 newDestination = Utils.GetRandomPosition(Ga.me.LevelMan.spawnArea);
//                         loco.Br.agent.destination = newDestination;
//                         _timerGeneral = 0f;
//                     }
//                 }
//                 break;
//
//             case Behavior.Patrol:
//                 Patrol();
//                 void Patrol()
//                 {
//                     if (OvrMove) return;
//                     if (Ga.me.waypoints == null || Ga.me.waypoints.Length == 0)
//                     {
//                         BehCurrent = Behavior.Stationary;
//                         return;
//                     }
//
//                     _timerGeneral += Time.deltaTime;
//                     if (loco.Br.agent.remainingDistance <= 0.5f || _timerGeneral > 10f)
//                     {
//                         loco.Br.agent.destination = Ga.me.waypoints[_counterWaypoints].position;
//                         _counterWaypoints = (1 + _counterWaypoints) % Ga.me.waypoints.Length;
//                         _timerGeneral = 0f;
//                     }
//                 }
//                 break;
//
//             case Behavior.Follow:
//                 Follow();
//                 void Follow()
//                 {
//                     if (OvrMove) return;
//                     if (Utils.Distance(loco.Br.myTransform.position, FollowTarget().position) > CONST_FollowDistance)  loco.Br.agent.destination = FollowTarget().position;
//                     else
//                     {
//                         if (loco.Br.agent.hasPath) loco.Br.agent.ResetPath();
//                         stationary();
//                     }
//                 }
//                 break;
//
//             case Behavior.Chase:
//                 Chase();
//                 void Chase()
//                 {
//                     if (OvrMove) return;
//                     if (loco.Br.combat.MyTarget == null) return;
//                     if (weaponRange == RangeArea.OutOfRange)
//                     {
//                         loco.Br.agent.destination = loco.Br.combat.MyTarget.position;
//                     }
//                     else if (loco.Br.agent.hasPath) loco.Br.agent.ResetPath();
//                 }
//
//                 break;
//
//             case Behavior.Flee:
//                 Flee();
//                 void Flee()
//                 {
//                     if (OvrMove) return;
//                     if (loco.Br.combat.MyTarget == null) return;
//                     if (loco.Br.combat.distanceToTarget < CONST_FleeDistance)
//                     {
//                         Vector3 direction = loco.Br.myTransform.position - loco.Br.combat.MyTarget.position;
//                         direction.y = 0f;
//                         direction.Normalize();
//                         Vector3 targetPosition = loco.Br.myTransform.position + 2f * direction;
//                         NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas);
//                         if (!hit.hit) return;
//                         loco.Br.agent.destination = hit.position;
//                     }
//                 }
//                 break;
//         }
//
//         defineAttackAnimation();
//
//         bool canMove = _canMoveNavigation && _canMoveCombat && IsAttackAnimationOver();
//         Toggle_Move(canMove);
//         if (!OvrMove) loco.Br.agent.speed = canMove ? moveSpeed : 0;
//
//         void stationary() 
//         {
//             _canMoveNavigation = false;
//
//             if (_stationaryIsTurning) loco.Br.myTransform.Rotate(_stationaryRotAxis, _timerStationary * 0.2f);
//         
//             _timerStationary += Time.deltaTime;
//             if (_timerStationary >= _timerStationaryMaxTime)
//             {
//                 _timerStationary = 0f;
//                 _stationaryRotAxis = Random.value > 0.5f ? Vector3.up : Vector3.down;
//                 _timerStationaryMaxTime = _stationaryIsTurning ? StationaryTimeIdle() : StationaryTimeRotating();
//                 _stationaryIsTurning = !_stationaryIsTurning;
//             }
//         }
//         void defineAttackAnimation()
//         {
//             bool[] attacks = new bool[2];
//             switch (weaponRange)
//             {
//                 case RangeArea.Melee:
//                     attacks[0] = true;
//                     break;
//                 case RangeArea.Ranged:
//                     attacks[1] = true;
//                     break;
//                 case RangeArea.OutOfRange:
//                     _canMoveCombat = true;
//                     break;
//             }
//             for (int i = 0; i < attacks.Length; i++)
//             {
//                 AttackAnimation(attacks[i], i);
//             }
//         }
//     }
//
//
//     void OnDrawGizmos()
//     {
//         if (!Application.isPlaying) return;
//         Gizmos.color = Color.red;
//         Gizmos.DrawSphere(loco.Br.agent.destination, 0.2f);
//     }
//
// }

