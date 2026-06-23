using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;


public class Loco : EventBus, IInit
{
    public virtual Brain Br { get; set; }

    public bool IsInitialized { get; set; } //only called in children (because they're on scene)
    [SerializeField] protected Animator anim;
    [SerializeField] protected MultiRotationConstraint rotationConstraint;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float knockBackResistance;
    [ReadOnly] public bool lookAtTarget;
    protected bool controlsEnabled = true;
    
    #region ANIMATOR
    int _moveHor = Animator.StringToHash("moveHor");
    int _moveVer = Animator.StringToHash("moveVer");
    int _walk = Animator.StringToHash("walk");
    int _attack = Animator.StringToHash("attack");
    int _attack1 = Animator.StringToHash("attack1");
    int _cast = Animator.StringToHash("cast");
    int _hit = Animator.StringToHash("hit");
    int _block = Animator.StringToHash("block");
    public void AE_Attack(int  num) => Br.combat.FromAnimEv_Attack(num);

    public void AE_Ultimate(int num) => Br.combat.FromAnimEv_Ultimate(num);

    protected void Direction_Move(float hor, float ver)
    {
        anim.SetFloat(_moveHor, hor);
        anim.SetFloat(_moveVer, ver);
    }
    protected void Toggle_Move(bool isMoving) => anim.SetBool(_walk, isMoving);
    public void AttInputEnemy(bool isAttacking) =>  anim.SetBool(_attack, isAttacking);
    public void Att1InputEnemy(bool isAttacking1) => anim.SetBool(_attack1, isAttacking1);
    public void CastSpell() =>  anim.SetTrigger(_cast);
    public void Hit() =>  anim.SetTrigger(_hit);
    public void Block() =>  anim.SetTrigger(_block);
    #endregion

    #region TOOLS
    public void KnockBack(Vector3 dir, int intensity = 1)
    {
        float diff = intensity - knockBackResistance;
        if (diff <= 0.5f) return;
        if (controlsEnabled) StartCoroutine(PushMeSequence(dir, diff));
    }
    protected virtual IEnumerator PushMeSequence(Vector3 dir, float deltaIntensity = 1) 
    {
        yield break;
    }
    
    // IEnumerator PushMeSequence(Vector3 dir, float intensity = 1)
    // {
    //     ControlsEnabled(false);
    //     if (dir == Vector3.zero)
    //     {
    //         dir = Utils.MakeV3(Random.insideUnitCircle);
    //     }
    //     Br.myRigid.AddForce(intensity * 10 * dir, ForceMode.VelocityChange);
    //     yield return new WaitForSeconds(Ga.me.gameData.pushDuration);
    //     ControlsEnabled(true);
    // }
    
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
        if (lookAtTarget && Br.combat.MyTarget != null)
        {
            faceDirection = Br.combat.MyTarget.position - Br.myTransform.position;
            faceDirection.y = 0f;
        }
        if (!faceDirection.Equals(Vector3.zero)) Br.myTransform.forward = faceDirection.normalized;
        
    }
    #endregion

    
}

