using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class P_Loco : Loco
{
    [SerializeField] ParticleSystem weaponTrail;
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            Ga.me.team.playerTransform = value.myTransform;
            value.agent.updateRotation = false;

            IsInitialized = true;
        }
    }
    public override bool OvrOrientation
    {
        get => base.OvrOrientation;
        set
        {
            base.OvrOrientation = value;
            if (value && joystickLookAt) OvrOrientation = false;
        }
    }
    public Alertness Disp
    {
        set
        {
            if (value == _alertness) return;
            _alertness = value;
            anim.SetLayerWeight(1, 1);
            rotationConstraint.weight = 0; 
           // if (weaponTrail != null) weaponTrail.Stop();
           OvrOrientation = true;
            switch (_alertness)
            {
                case Alertness.Relaxed:
                    AttackAnimation(null);              
                    OvrOrientation = false;
                    anim.SetLayerWeight(1, 0);
                    break;
                case Alertness.Alarmed:
                    AttackAnimation(null);              
                    break;
                case Alertness.Fighting:
                  //  if (weaponTrail != null) weaponTrail.Play();
                    AttackAnimation(AnimAttackType.Melee);
                    rotationConstraint.weight = 1;
                    break;
            }

        }
    }
    [ShowInInspector, ReadOnly] Alertness _alertness;
    [HideInInspector] public Vector2 effJoystickValue;
    [SerializeField] bool joystickLookAt = true;
    int _posId = Shader.PropertyToID("_Position");
    int _sizeID = Shader.PropertyToID("_Size");
    Transform _camTransform;

    void Awake()
    {
        _camTransform = Ga.me.cam.transform;
    }

    void Update()
    {
        float camAngle = Ga.me.cameraRigTransform.eulerAngles.y;
        effJoystickValue = Quaternion.Euler(0, 0, -camAngle) * Ga.me.joystick.value;
        Vector3 myForward;
        if (OvrOrientation && Br.combat.MyTarget != null) myForward = Utils.Direction(Br.myTransform.position, Br.combat.MyTarget.position);
        else myForward = Utils.MakeV3(effJoystickValue);
        Orientation(myForward);
        if (!OvrMove) Move();
        void Move()
        {
            float dotVer = Vector3.Dot(Utils.MakeV3(effJoystickValue), Br.myTransform.forward);
            float dotHor = Vector3.Dot(Utils.MakeV3(effJoystickValue), Br.myTransform.right);
            Direction_Move(dotHor, dotVer);
            Br.agent.velocity = moveSpeed * Utils.MakeV3(effJoystickValue);
        }
    }

    void LateUpdate()
    {
        Utils.CameraFollowAsymptotic(Br.myTransform.position, Ga.me.cameraRigTransform);
    }

}
