using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class PlControl : EventBus, ICharacter
{
    [field: SerializeField] public Transform MyTransform { get; set; }
    public Transform MyTarget { get; set; }
    [SerializeField] float moveSpeed;
    [Title("References")]
    [SerializeField] Health health;
    [SerializeField] PlAnim plAnim;
    [SerializeField] Rigidbody myRigid;
    [SerializeField] PlProfession profession;
    [SerializeField] PlProjectileBody playerProjectileBody;
    InputAction _inputAttack;

    float _velocityModifier = 1f;
    bool _isDashing;
    const int CONST_DashVelocityMax = 80;

    protected override void OnEnable()
    {
        base.OnEnable();
        _inputAttack = InputSystem.actions.FindAction("Player/Jump");
        _inputAttack.Enable();
    }

    void Start()
    {
        health.InitializeMe(this);
        profession.InitializeMe(this);
        plAnim.InitializeMe(profession);
        playerProjectileBody.IsActive = false;
    }

    public IEnumerator Dash()
    {
        _isDashing = true;
        _velocityModifier = 1f;
        float duration = 0.2f;
        playerProjectileBody.IsActive = true;
        while (_velocityModifier > 0)
        {
            _velocityModifier -= Time.deltaTime / duration;
            yield return null;
        }
        playerProjectileBody.IsActive = false;
        _velocityModifier = 1f;
        _isDashing = false;
    }
    void FixedUpdate()
    {
        float camAngle = gm.cameraRigTransform.eulerAngles.y;
        Vector2 val = Quaternion.Euler(0, 0, -camAngle) * gm.joystick.value;
        plAnim.MoveInput(moveSpeed * val.sqrMagnitude != 0);
        
        Vector3 finalVelocity;
        if (_isDashing)
        {
            finalVelocity = _velocityModifier * CONST_DashVelocityMax * MyTransform.forward;
        }
        else
        {
            finalVelocity = _velocityModifier * Utils.MakeV3(moveSpeed * val);
            LookAtMethod();
        }
        myRigid.linearVelocity = finalVelocity;
        

    }

    void LookAtMethod()
    {
        Transform closestEnemy = Utils.ClosestTransform(MyTransform.position, gm.allEnemies);
        Vector3 faceDirection = Vector3.forward;
        if (closestEnemy != null)
        {
            faceDirection = closestEnemy.position - MyTransform.position;
            faceDirection.y = 0f;
        }
        MyTransform.forward = faceDirection.normalized;
        
    }

}
