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
            agent.updateRotation = false;

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
            AttackAnimation(false);
            AttackAnimation(false, 1);
            rotationConstraint.weight = 0; 
           // if (weaponTrail != null) weaponTrail.Stop();
           OvrOrientation = true;
            switch (_alertness)
            {
                case Alertness.Relaxed:
                    OvrOrientation = false;
                    anim.SetLayerWeight(1, 0);
                    break;
                case Alertness.Alarmed:
                    break;
                case Alertness.Fighting:
                  //  if (weaponTrail != null) weaponTrail.Play();
                    AttackAnimation(true);
                    AttackAnimation(true, 1);
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
    public NavMeshAgent agent;

    void Awake()
    {
        _camTransform = Ga.me.cam.transform;
    }

    void Update()
    {
        float camAngle = Ga.me.cameraRigTransform.eulerAngles.y;
        effJoystickValue = Quaternion.Euler(0, 0, -camAngle) * Ga.me.joystick.value;
        Vector3 myForward = Vector3.zero;
        if (OvrOrientation && Br.combat.MyTarget != null) myForward = Utils.Direction(Br.myTransform.position, Br.combat.MyTarget.position);
        else myForward = Utils.MakeV3(effJoystickValue);
        Orientation(myForward);
        Utils.CameraFollowAsymptotic(Br.myTransform.position, Ga.me.cameraRigTransform);
    }

    void FixedUpdate()
    {
        if (!OvrMove) Move();
        
        float shaderFloat = Physics.Linecast(_camTransform.position, Br.myTransform.position, Utils.MyLayer(Ga.me.gameData.layWallsSeeThrough)) ? 0.5f: 0f;
        Ga.me.matSeeThroughWalls.SetFloat(_sizeID, shaderFloat);

        void Move()
        {
            float dotVer = Vector3.Dot(Utils.MakeV3(effJoystickValue), Br.myTransform.forward);
            float dotHor = Vector3.Dot(Utils.MakeV3(effJoystickValue), Br.myTransform.right);
            Direction_Move(dotHor, dotVer);
          //  Br.myRigid.AddForce(1000 * Utils.MakeV3(moveSpeed * effJoystickValue));
          agent.velocity = Utils.MakeV3(moveSpeed * effJoystickValue);
        }

    }

    protected override void CallEv_OnLevelLoaded()
    {
        base.CallEv_OnLevelLoaded();
        Physics.IgnoreCollision(Ga.me.LevelMan.ground.GetComponent<Collider>(), Br.myCollider);
    }

    protected override IEnumerator PushMeSequence(Vector3 dir, float deltaIntensity = 1)
    {
        yield return base.PushMeSequence(dir, deltaIntensity);
        OvrMove = true;
        float effIntensity = 5 * deltaIntensity;
        effIntensity = Mathf.Clamp(effIntensity, 0f, 30f);
        Br.myRigid.AddForce(effIntensity * dir, ForceMode.VelocityChange);
        yield return new WaitForSeconds(Ga.me.gameData.pushDuration);
        OvrMove = false;
    }
}
