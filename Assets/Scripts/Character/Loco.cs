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
    [SerializeField]  GameObject iconWait;
    [SerializeField] LineRenderer line;


    #region ANIMATOR
    
    public void AE_Attack(int  num)
    {
        Br.combat.FromAnimEv_Attack(num);
    }

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
    protected void Att1InputEnemy(bool isAttacking1)
    {
        anim.SetBool("attack1", isAttacking1);
    }
    #endregion

    #region TOOLS
    public void Dash(int intensity = 1)
    {
        KnockBack(Br.myTransform.forward, intensity, true);
        // StartCoroutine(Delay());
        // IEnumerator Delay()
        // {
        //     Vector3 startPos = Br.myRigid.transform.position;
        //     Br.myRigid.isKinematic = true;
        //     Br.myRigid.MovePosition(Br.myTransform.position + intensity * Br.myTransform.forward);
        //     yield return new WaitForFixedUpdate();
        //     Vector3 endPos = Br.myRigid.transform.position;
        //     Br.myRigid.isKinematic = false;
        //     Br.myRigid.linearVelocity = Vector3.zero;
        //     
        //     Collider[] colliders = Physics.OverlapCapsule(startPos, endPos, 1);
        //     foreach (Collider col in colliders)
        //     {
        //         if (col.TryGetComponent(out Brain br) &&  
        //             Utils.RelationFactions(Br.faction, br.faction) == Relation.Hostile &&
        //             br.loco!= null)
        //         {
        //             print(br.name);
        //         }
        //
        //     }
        //     line.SetPosition(0, startPos + 0.1f * Vector3.up);
        //     line.SetPosition(1, endPos + 0.1f * Vector3.up);
        //     line.enabled = true;
        // }
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


    protected virtual void ControlsEnabled(bool isEnabled)
    {
       //iconWait.SetActive(!isEnabled);
    }
    
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

// IEnumerator PushMeSequence(Vector3 dir, float intensity)
// {
//     ControlsEnabled(false);
//     float duration = 0.2f;
//     float pushPower = 20 * intensity;
//     velocityModifier = 1f;
//     bool isPlayer = Ga.me.playerTransform == Br.myTransform;
//     while (velocityModifier > 0f)
//     {
//         velocityModifier -= Time.deltaTime / duration;
//         Br.myRigid.linearVelocity = velocityModifier * pushPower * dir;
//         yield return null;
//     }
//     ControlsEnabled(true);
//     velocityModifier = 1f;
//     bodyProjectile.IsActive = false;
//     controlledFromOutside = false;
// }

