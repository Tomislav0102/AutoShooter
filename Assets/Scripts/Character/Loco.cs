using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Loco : EventBus, IInit
{
    protected Brain br;
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

    public virtual void Initialize(Brain brain)
    {
        br = brain;
        bodyProjectile.IsActive = false;
    }

    protected virtual void Update()
    {
        anim.SetLayerWeight(1, br.combat.isAttacking ? 1 : 0);
        rotationConstraint.weight = br.combat.isAttacking? 1 : 0;
    }
    
    public void AE_Attack(int  num)
    {
        br.combat.AE_Attack(num);
    }
    public void MoveInputPlayer(float hor, float ver)
    {
        anim.SetFloat("moveHor", hor);
        anim.SetFloat("moveVer", ver);
    }
    public void MoveInputEnemy(bool move)
    {
        anim.SetBool("walk", move);
    }
    public void AttInputEnemy(bool attack)
    {
        anim.SetBool("attack", attack);
    }
    public void Att1InputEnemy(bool attack)
    {
        anim.SetBool("attack1", attack);
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
            br.myRigid.isKinematic = false;
            br.myRigid.collisionDetectionMode = CollisionDetectionMode.Continuous;
            float duration = 0.2f;
            float pushPower = 80 * intensity;
            Vector3 dir = br.loco.myTransform.position - origin;
            dir.y = 0;
            dir.Normalize();
            velocityModifier = 1f;
            while (velocityModifier > 0f)
            {
                velocityModifier -= Time.deltaTime / duration;
                br.myRigid.linearVelocity = velocityModifier * pushPower * dir;
                yield return null;
            }
            br.myRigid.linearVelocity = Vector3.zero;
            br.myRigid.isKinematic = true;
            br.myRigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
            if (agent != null) agent.enabled = true;
        }
    }
    
    protected void LookAtMethod(HashSet<Transform> targets)
    {
        Transform closestEnemy = Utils.ClosestTransform(myTransform.position, targets);
        Vector3 faceDirection = Vector3.forward;
        if (closestEnemy != null)
        {
            faceDirection = closestEnemy.position - myTransform.position;
            faceDirection.y = 0f;
        }
        myTransform.forward = faceDirection.normalized;
    }


}
