using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
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
    public bool IsReady { get; set; } //only called in children (because they're on scene)
    [SerializeField] protected Animator anim;
    [SerializeField] MultiRotationConstraint rotationConstraint;
    [SerializeField] protected float moveSpeed;
    [SerializeField] BodyProjectile bodyProjectile;
    Coroutine _pushCoroutine;


    protected virtual void Update()
    {
        if (Br.combat == null) return;
        anim.SetLayerWeight(1, Br.combat.isAttacking ? 1 : 0);
        rotationConstraint.weight = Br.combat.isAttacking? 1 : 0;
    }

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
    protected void Att1InputEnemy(bool isAttacking1)
    {
        anim.SetBool("attack1", isAttacking1);
    }
    #endregion

    #region TOOLS
    
    public void Dash(Vector3 dir, float intensity = 1f)
    {
        bodyProjectile.IsActive = true;
        KnockBack(dir, intensity);
    }
    public void KnockBack(Vector3 dir, float intensity = 1f)
    {
        if (_pushCoroutine != null) StopCoroutine(_pushCoroutine);
        _pushCoroutine = StartCoroutine(PushMeSequence(dir, intensity));
    }
    IEnumerator PushMeSequence(Vector3 dir, float intensity)
    {
        ControlsEnabled(false);
        Br.myRigid.AddForce(intensity * 1000 * dir, ForceMode.Impulse);
        yield return new WaitForSeconds(0.2f);
        ControlsEnabled(true);
        bodyProjectile.IsActive = false;
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

    protected virtual void ControlsEnabled(bool isEnabled) { }
    protected void LookAtMethod()
    {
        Vector3 faceDirection = Vector3.forward;
        if (Br.combat.MyTarget != null)
        {
            faceDirection = Br.combat.MyTarget.position - Br.myTransform.position;
            faceDirection.y = 0f;
        }
        if (!faceDirection.Equals(Vector3.zero)) Br.myTransform.forward = faceDirection.normalized;
    }
    #endregion

    
}
