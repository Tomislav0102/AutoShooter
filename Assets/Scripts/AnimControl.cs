using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimControl : MonoBehaviour
{
    PlayerControl _playerControl;
    Animator _anim;
    public bool isShooting;
    [SerializeField] MultiRotationConstraint rotationConstraint;

    public void InitializeMe(PlayerControl playerControl)
    {
        _playerControl = playerControl;
        _anim = GetComponent<Animator>();
    }


    void Update()
    {
        _anim.SetLayerWeight(1, isShooting ? 1 : 0);
        rotationConstraint.weight = isShooting ? 1 : 0;
    }

    public void AE_Attack()
    {
      //  print("attacked");
      if (isShooting) _playerControl.AttackAnimEvent();
    }

    public void MoveInput(bool isMoving)
    {
        _anim.SetBool("isMoving", isMoving);
    }
        
}
