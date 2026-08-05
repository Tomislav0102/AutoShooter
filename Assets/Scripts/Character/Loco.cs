using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class Loco : MonoBehaviour, IInitialization
{
    [SerializeField] UnityEvent<Brain> brainEv;
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            OvrMotion = false;
            OvrOrientation = false;
            _avoidancePriorityDefault = value.agent.avoidancePriority;
            brainEv?.Invoke(value);
        }
    }
    Brain _br;
    public Animator anim;
    [SerializeField, Range(0, 10)] public int moveSpeed = 1;
    [SerializeField] protected int knockBackResistance;
    [field: SerializeField] public bool OvrMotion { get; set; } //can control player, override agent destination

    public bool OvrOrientation //has player joystickLookAt, agent.updateRotation
    {
        get => _ovrOrientation;
        set
        {
            _ovrOrientation = value;
            if (value && isOrientationAlwaysFalse) OvrOrientation = false;
        }
    }
    bool _ovrOrientation;
    [SerializeField] protected bool isOrientationAlwaysFalse = true;
    Coroutine _pushCoroutine;
    public enum MoveOverrideType { None, KnockBack, Dash, Magnet }
    MoveOverrideType _currentMoveOverride = MoveOverrideType.None;
    int _avoidancePriorityDefault;
    
    #region ANIMATOR
    int _moveHor = Animator.StringToHash("moveHor");
    int _moveVer = Animator.StringToHash("moveVer");
    int _walk = Animator.StringToHash("walk");
    int _attMelee = Animator.StringToHash("melee");
    int _attRanged = Animator.StringToHash("ranged");
    int _cast = Animator.StringToHash("cast");
    int _hit = Animator.StringToHash("hit");
    int _block = Animator.StringToHash("block");
    int _roll = Animator.StringToHash("roll");
    
    public bool IsAttackAnimationPlaying() => anim.GetCurrentAnimatorStateInfo(0 ).IsTag("Attacks");
    public void AE_Attack(int num) => Br.combat.FromAnimEv_Attack(num);
    public void AE_Ultimate(int num) => Br.combat.FromAnimEv_Ultimate(num);
    public void Direction_Move(float hor, float ver)
    {
        anim.SetFloat(_moveHor, hor);
        anim.SetFloat(_moveVer, ver);
    }
    public void Toggle_Move(bool isMoving) => anim.SetBool(_walk, isMoving);
    public void AttackAnimation(AnimAttackType? attackType)
    {
        if (attackType == null)
        {
            anim.SetBool(_attMelee, false);
            anim.SetBool(_attRanged, false);
            return;
        }
        
        switch (attackType)
        {
            case AnimAttackType.Melee:
            anim.SetBool(_attRanged, false);
                anim.SetBool(_attMelee, true);
                break;
            case AnimAttackType.Ranged:
            anim.SetBool(_attMelee, false);
                anim.SetBool(_attRanged, true);
                break;
            case AnimAttackType.Ultimate:
                anim.SetTrigger(_cast);
                break;
        }
    }
    public void Roll() => anim.SetTrigger(_roll);
    public void Hit() => anim.SetTrigger(_hit);
    public void Block() => anim.SetTrigger(_block);
    #endregion

    #region TOOLS
    public void Orientation(Vector3 lookAtDirection)
    {
        lookAtDirection.y = 0;
        OrientationFinal(lookAtDirection);
    }
    public void Orientation(Transform lookAtPosition)
    {
        if (lookAtPosition == null) return;
        Vector3 pos = new Vector3(lookAtPosition.position.x, 0f, lookAtPosition.position.z);
        OrientationFinal(Utils.Direction(Br.myTransform.position, pos));
    }
    void OrientationFinal(Vector3 look)
    {
        if (look.Equals(Vector3.zero)) return;
        
        //instant
        Br.myTransform.rotation = Quaternion.LookRotation(look);
        return;
        
        //animated
        Quaternion rot = Quaternion.LookRotation(look);
        Br.myTransform.rotation = Quaternion.RotateTowards(Br.myTransform.rotation, rot, Ga.me.gameData.agentRotSpeed * Time.deltaTime);
    }

    public void PushMe(Vector3 dir, MoveOverrideType moveOverrideType = MoveOverrideType.KnockBack, int intensity = 1)
    {
        float timer = 0f;
        switch (moveOverrideType)
        {
            case MoveOverrideType.KnockBack:
                if (_currentMoveOverride == MoveOverrideType.Dash) return;
                intensity -= knockBackResistance;
                if (intensity <= 0) return;
                timer = 0.2f;
                break;
            case MoveOverrideType.Dash:
              //  intensity = 5;
                timer = Ga.me.gameData.dashTime;
                break;
            case MoveOverrideType.Magnet:
                intensity -= knockBackResistance;
                if (intensity <= 0) return;
                break;
        }
        _currentMoveOverride =  moveOverrideType;
        
        if (_pushCoroutine != null) StopCoroutine(_pushCoroutine);
        _pushCoroutine = StartCoroutine(pushDelay());

        IEnumerator pushDelay()
        {
            OvrMotion = true;
            Br.agent.acceleration = 10;
            Br.agent.velocity = intensity * dir;
            _avoidancePriorityDefault = Br.agent.avoidancePriority;
            if (_currentMoveOverride == MoveOverrideType.Dash) Br.agent.avoidancePriority = 40;
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            Br.agent.acceleration = 10000;
            OvrMotion = false;
            if (_currentMoveOverride == MoveOverrideType.Dash) Br.agent.avoidancePriority = _avoidancePriorityDefault;
            _currentMoveOverride = MoveOverrideType.None;
        }
    }

    public void Magnet(Vector3 center, int intensity = 1)
    {
        if (intensity <= knockBackResistance) return;
        Vector2 vDelta = Utils.MakeV2(Br.myTransform.position) - Utils.MakeV2(center);
        if (vDelta.sqrMagnitude < 0.1f) return;
        StartCoroutine(attractDelay());

        IEnumerator attractDelay()
        {
            OvrMotion = true;
            yield break;
            OvrMotion = false;
        }
    }

    #endregion


}

