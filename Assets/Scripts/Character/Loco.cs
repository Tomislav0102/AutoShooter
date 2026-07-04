using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;


public class Loco : EventBus, IInit
{
    public virtual Brain Br
    {
        get => _br;
        set
        {
            _br = value;
        }
    }
    Brain _br;

    public bool IsInitialized { get; set; } //only called in children (because they're on scene)
    public virtual Impairment Impaired { get; set; }
    [SerializeField] protected Animator anim;
    [SerializeField] protected MultiRotationConstraint rotationConstraint;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float knockBackResistance;

    public enum Impairment
    {
        None,
        Move, //can control player, override agent destination
        Orientate, //has player joystickLookAt, agent.updateRotation
        Both
    }

    #region ANIMATOR
    int _moveHor = Animator.StringToHash("moveHor");
    int _moveVer = Animator.StringToHash("moveVer");
    int _walk = Animator.StringToHash("walk");
    int _attMelee = Animator.StringToHash("melee");
    int _attRanged = Animator.StringToHash("ranged");
    int _isAttacking = Animator.StringToHash("isAttacking");
    int _cast = Animator.StringToHash("cast");
    int _hit = Animator.StringToHash("hit");
    int _block = Animator.StringToHash("block");
    int _roll = Animator.StringToHash("roll");
    public void AE_Attack(int num) => Br.combat.FromAnimEv_Attack(num);

    public void AE_Ultimate(int num) => Br.combat.FromAnimEv_Ultimate(num);

    protected void Direction_Move(float hor, float ver)
    {
        anim.SetFloat(_moveHor, hor);
        anim.SetFloat(_moveVer, ver);
    }

    protected void Toggle_Move(bool isMoving) => anim.SetBool(_walk, isMoving);

    public void Attack(bool attack, int index = 0) => anim.SetBool(index == 0 ? _attMelee : _attRanged, attack);
    public void AttackDone() => anim.SetBool(_isAttacking, false);
    public void CastSpell() => anim.SetTrigger(_cast);
    public void Roll() => anim.SetTrigger(_roll);
    public void Hit() => anim.SetTrigger(_hit);
    public void Block() => anim.SetTrigger(_block);
    #endregion

    #region TOOLS
    public void MotionOverrideKnockBack(Vector3 dir, int intensity = 1)
    {
        float diff = intensity - knockBackResistance;
        if (diff <= 0.5f) return;
        if (dir == Vector3.zero) dir = Utils.MakeV3(Random.insideUnitCircle);
        StartCoroutine(PushMeSequence(dir, diff));
    }

    protected virtual IEnumerator PushMeSequence(Vector3 dir, float deltaIntensity = 1)
    {
        yield break;
    }

    public virtual void MotionOverrideMagnet(bool isOn, Vector3 center) { }

    protected void LookAtMethod()
    {
        LookAtMethod_Continue(Vector3.forward);
    }

    protected void LookAtMethod(Vector3 joystickValue)
    {
        LookAtMethod_Continue(joystickValue);
    }

    void LookAtMethod_Continue(Vector3 faceDirection)
    {
        if (Br.combat.MyTarget != null)
        {
            faceDirection = Utils.Direction(Br.myTransform.position, Br.combat.MyTarget.position);
        }
        if (!faceDirection.Equals(Vector3.zero)) Br.myTransform.forward = faceDirection.normalized;

    }
    #endregion


}

