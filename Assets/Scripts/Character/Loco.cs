using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Animations.Rigging;


public class StatProvider
{
    float _baseValue;
    public HashSet<float> mods;
    Dictionary<object, float> _stats;

    public float Value()
    {
        float fl = _baseValue;
        foreach (float item in mods)
        {
            fl += item;
        }
        return fl;
    }

    public StatProvider(float baseValue)
    {
        _baseValue = baseValue;
        mods = new HashSet<float>();
    }
}
public class Loco : EventBus, IInit
{
    public virtual Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            modMoveSpeed = new StatProvider(moveSpeed);
        }
    }
    Brain _br;
    public bool IsReady { get; set; } //only called in children (because they're on scene)
    [SerializeField] protected Animator anim;
    [SerializeField] protected MultiRotationConstraint rotationConstraint;
    [SerializeField] float moveSpeed;
    protected StatProvider modMoveSpeed;
    Coroutine _pushCoroutine;
    [SerializeField] LineRenderer line;


    #region ANIMATOR
    
    public void AE_Attack(int  num) => Br.combat.FromAnimEv_Attack(num);

    public void AE_SpellCast(int num) => Br.combat.FromAnimEv_SpellCast(num);

    protected void Direction_Move(float hor, float ver)
    {
        anim.SetFloat("moveHor", hor);
        anim.SetFloat("moveVer", ver);
    }
    protected void Toggle_Move(bool isMoving)
    {
        anim.SetBool("walk", isMoving);
    }
    protected void AttInputEnemy(bool isAttacking)
    {
        anim.SetBool("attack", isAttacking);
    }

    public void Hit() 
    {
        anim.SetTrigger("hit");
    }
    public void Block() 
    {
        anim.SetTrigger("block");
    }

    public void CastSpell()
    {
        anim.SetTrigger("cast");
    }
    protected void Att1InputEnemy(bool isAttacking1)
    {
        anim.SetBool("attack1", isAttacking1);
    }
    #endregion

    #region TOOLS
    public void Dash(int intensity = 1)
    {
        KnockBack(Br.myTransform.forward, intensity, true);
    }
    public void KnockBack(Vector3 dir, int intensity = 1, bool isDashing = false)
    {
        if (_pushCoroutine != null) StopCoroutine(_pushCoroutine);
        _pushCoroutine = StartCoroutine(PushMeSequence(dir, intensity, isDashing));
    }
    IEnumerator PushMeSequence(Vector3 dir, int intensity = 1, bool isDashing = false)
    {
        ControlsEnabled(false);
        Br.myRigid.AddForce(intensity * 10 * dir, ForceMode.VelocityChange);
        yield return new WaitForSeconds(Ga.me.gameData.pushDuration);
        ControlsEnabled(true);
    }


    protected virtual void ControlsEnabled(bool isEnabled) { }
    
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
            faceDirection = Br.combat.MyTarget.position - Br.myTransform.position;
            faceDirection.y = 0f;
        }
        if (!faceDirection.Equals(Vector3.zero)) Br.myTransform.forward = faceDirection.normalized;
        
    }
    #endregion

    
}

