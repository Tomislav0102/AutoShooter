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
            OvrMove = false;
            OvrOrientation = false;
        }
    }
    Brain _br;

    public bool IsInitialized { get; set; } //only called in children (because they're on scene)
    [SerializeField] protected Animator anim;
    [SerializeField] protected MultiRotationConstraint rotationConstraint;
    [SerializeField, Range(0, 10)] protected int moveSpeed = 1;
    [SerializeField] protected int knockBackResistance;
    [field:SerializeField] public virtual bool OvrMove { get; set; } //can control player, override agent destination
    [field:SerializeField] public virtual bool OvrOrientation { get; set; } //has player joystickLookAt, agent.updateRotation
    Coroutine _pushCoroutine, _magnetCoroutine;
    
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
    public void AE_Attack(int num) => Br.combat.FromAnimEv_Attack(num);

    public void AE_Ultimate(int num) => Br.combat.FromAnimEv_Ultimate(num);

    protected void Direction_Move(float hor, float ver)
    {
        anim.SetFloat(_moveHor, hor);
        anim.SetFloat(_moveVer, ver);
    }

    protected void Toggle_Move(bool isMoving) => anim.SetBool(_walk, isMoving);

    public void AttackAnimation(bool attack, int index = 0) 
    {
        if (Br.debugGeneral) print(attack);
        anim.SetBool(index == 0 ? _attMelee : _attRanged, attack);
    }
    public void CastSpell() => anim.SetTrigger(_cast);
    public void Roll() => anim.SetTrigger(_roll);
    public void Hit() => anim.SetTrigger(_hit);
    public void Block() => anim.SetTrigger(_block);
    #endregion


    
    #region TOOLS
    protected void Orientation(Vector3 lookAtDirection)
    {
        lookAtDirection.y = 0;
        OrientationFinal(lookAtDirection);
    }
    protected void Orientation(Transform lookAtPosition)
    {
        if (lookAtPosition == null) return;
        Vector3 pos = new Vector3(lookAtPosition.position.x, 0f, lookAtPosition.position.z);
        OrientationFinal(Utils.Direction(Br.myTransform.position, pos));
    }
    void OrientationFinal(Vector3 look)
    {
        if (look.Equals(Vector3.zero)) return; 
        Br.myTransform.rotation = Quaternion.LookRotation(look);
        // Quaternion rot = Quaternion.LookRotation(lookAtTarget);
        // Br.myTransform.rotation = Quaternion.RotateTowards(Br.myTransform.rotation, rot, Ga.me.gameData.agentRotSpeed * Time.deltaTime);
    }


    public void KnockBack(Vector3 dir, int intensity = 1)
    {
        float diff = intensity - knockBackResistance;
        if (diff <= 0.5f) return;
        
        if (dir == Vector3.zero) dir = Utils.MakeV3(Random.insideUnitCircle);
        if (_pushCoroutine != null) StopCoroutine(_pushCoroutine);
        _pushCoroutine = StartCoroutine(PushMe());
        IEnumerator PushMe()
        {
            OvrMove = true;
            float effIntensity = 5 * diff;
            effIntensity = Mathf.Clamp(effIntensity, 0f, 10f);
            Vector3 velocity = effIntensity * dir;
            Br.agent.ResetPath();
            while (velocity.magnitude > 0.2f)
            {
                Br.agent.velocity = velocity;
                velocity = Vector3.MoveTowards(velocity, Vector3.zero, effIntensity * Time.deltaTime);
                yield return null;
            }
            OvrMove = false;
        }
    }    


    public void Magnet(Vector3 center, int intensity = 1)
    {
        if (intensity <= knockBackResistance) return;
        Vector2 vDelta = Utils.MakeV2(Br.myTransform.position) - Utils.MakeV2(center);
        if (vDelta.sqrMagnitude < 0.1f) return;
        if (_magnetCoroutine != null) StopCoroutine(_magnetCoroutine);
        _magnetCoroutine = StartCoroutine(AttractMe());
        IEnumerator AttractMe()
        {
            OvrMove = true;
            yield break;
            OvrMove = false;
        }
    }

    #endregion


}

