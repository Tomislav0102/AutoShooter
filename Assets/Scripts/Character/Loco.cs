using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Loco : EventBus, IInit
{
    public virtual Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            bodyProjectile.IsActive = false;
        }
    }
    Brain _br;
    /// <summary>
    /// for now it's assigned to Brain
    /// NavMesh agent also on Brain
    /// </summary>
    public Transform myTransform;
    public bool IsReady { get; set; } //only called in children (because they're on scene)
    [SerializeField] protected Animator anim;
    [SerializeField] MultiRotationConstraint rotationConstraint;
    [SerializeField] protected float moveSpeed;
    [SerializeField] BodyProjectile bodyProjectile;
    protected float velocityModifier = 1f;
    protected bool isDashing;
    protected const int CONST_DASH_VELOCITY_MAX = 80;
    Coroutine _pushCoroutine;


    protected virtual void Update()
    {
        if (Br.combat == null) return;
        anim.SetLayerWeight(1, Br.combat.isAttacking ? 1 : 0);
        rotationConstraint.weight = Br.combat.isAttacking? 1 : 0;
    }
    
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

    public void Hit() => anim.SetTrigger("hit");
    protected void Att1InputEnemy(bool isAttacking1)
    {
        anim.SetBool("attack1", isAttacking1);
    }
    
    public IEnumerator Dash()
    {
        isDashing = true;
        velocityModifier = 1f;
        float duration = 0.2f;
        bodyProjectile.IsActive = true;
        while (velocityModifier > 0)
        {
            velocityModifier -= Time.deltaTime / duration;
            yield return null;
        }
        bodyProjectile.IsActive = false;
        velocityModifier = 1f;
        isDashing = false;
    }

    public void PushMe(Vector3 origin, NavMeshAgent agent = null, float intensity = 1f)
    {
        if (_pushCoroutine != null) StopCoroutine(_pushCoroutine);
        _pushCoroutine = StartCoroutine(PushMeSequence());
            
        IEnumerator PushMeSequence()
        {
            if (agent != null) agent.enabled = false;
            Br.myRigid.isKinematic = false;
            Br.myRigid.collisionDetectionMode = CollisionDetectionMode.Continuous;
            float duration = 0.2f;
            float pushPower = 80 * intensity;
            Vector3 dir = Br.loco.myTransform.position - origin;
            dir.y = 0;
            dir.Normalize();
            velocityModifier = 1f;
            while (velocityModifier > 0f)
            {
                velocityModifier -= Time.deltaTime / duration;
                Br.myRigid.linearVelocity = velocityModifier * pushPower * dir;
                yield return null;
            }
            Br.myRigid.linearVelocity = Vector3.zero;
            Br.myRigid.isKinematic = true;
            Br.myRigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
            if (agent != null) agent.enabled = true;
        }
    }
    
    protected void LookAtMethod()
    {
        Vector3 faceDirection = Vector3.forward;
        if (Br.combat.MyTarget != null)
        {
            faceDirection = Br.combat.MyTarget.position - myTransform.position;
            faceDirection.y = 0f;
        }
        if (!faceDirection.Equals(Vector3.zero)) myTransform.forward = faceDirection.normalized;
    }

}
