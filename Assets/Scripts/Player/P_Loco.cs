using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class P_Loco : EventBus, ILocomotion
{
    Brain _brain;
    InputAction _inputAttack;

    float _velocityModifier = 1f;
    bool _isDashing;
    const int CONST_DashVelocityMax = 80;

    protected override void Awake()
    {
        base.Awake();
        _brain = GetComponent<Brain>();
    }

    public void Initialize(Brain brain)
    {
         _brain = brain;
         IsReady = true;
    }

    public bool IsReady { get; set; }

    protected override void OnEnable()
    {
        base.OnEnable();
        _inputAttack = InputSystem.actions.FindAction("Player/Jump");
        _inputAttack.Enable();
    }

    void Start()
    {
        _brain.playerProjectileBody.IsActive = false;
    }

    public IEnumerator Dash()
    {
        _isDashing = true;
        _velocityModifier = 1f;
        float duration = 0.2f;
        _brain.playerProjectileBody.IsActive = true;
        while (_velocityModifier > 0)
        {
            _velocityModifier -= Time.deltaTime / duration;
            yield return null;
        }
        _brain.playerProjectileBody.IsActive = false;
        _velocityModifier = 1f;
        _isDashing = false;
    }
    void FixedUpdate()
    {
        float camAngle = gm.cameraRigTransform.eulerAngles.y;
        Vector2 val = Quaternion.Euler(0, 0, -camAngle) * gm.joystick.value;
        float dotVer = Vector3.Dot(Utils.MakeV3(val), _brain.MyTransform.forward);
        float dotHor = Vector3.Dot(Utils.MakeV3(val), _brain.MyTransform.right);
        _brain.animHub.MoveInput(dotHor, dotVer);
        
        Vector3 finalVelocity;
        if (_isDashing)
        {
            finalVelocity = _velocityModifier * CONST_DashVelocityMax * _brain.MyTransform.forward;
        }
        else
        {
            finalVelocity = _velocityModifier * Utils.MakeV3(_brain.moveSpeed * val);
            LookAtMethod();
        }
        _brain.myRigid.linearVelocity = finalVelocity;
        
    }
    void LookAtMethod()
    {
        Transform closestEnemy = Utils.ClosestTransform(_brain.MyTransform.position, gm.allEnemies);
        Vector3 faceDirection = Vector3.forward;
        if (closestEnemy != null)
        {
            faceDirection = closestEnemy.position - _brain.MyTransform.position;
            faceDirection.y = 0f;
        }
        _brain.MyTransform.forward = faceDirection.normalized;
    }

}
