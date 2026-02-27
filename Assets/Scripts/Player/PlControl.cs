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
    InputAction _inputAttack;


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
    }

    void FixedUpdate()
    {
        float camAngle = gm.cameraRigTransform.eulerAngles.y;
        Vector2 val = Quaternion.Euler(0, 0, -camAngle) * gm.joystick.value;
        myRigid.linearVelocity = Utils.MakeV3(moveSpeed * val);
        plAnim.MoveInput(moveSpeed * val.sqrMagnitude != 0);
    }



    public void AttackAnimEvent()
    {
    }

}
