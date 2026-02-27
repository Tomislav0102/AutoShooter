using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlAnim : MonoBehaviour
{
    PlProfession _profession;
    Animator _anim;
    [SerializeField] MultiRotationConstraint rotationConstraint;

    public void InitializeMe(PlProfession profession)
    {
        _profession = profession;
        _anim = GetComponent<Animator>();
    }


    void Update()
    {
        _anim.SetLayerWeight(1, _profession.isAttacking ? 1 : 0);
        rotationConstraint.weight = _profession.isAttacking ? 1 : 0;
    }

    public void AE_Attack()
    {
        _profession.AttackAnimEvent();
    }

    public void MoveInput(bool isMoving)
    {
        _anim.SetBool("isMoving", isMoving);
    }
        
}
