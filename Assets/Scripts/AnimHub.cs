using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimHub : MonoBehaviour, IInit
{
    Brain _brain;
    public bool IsReady { get; set; }
    [SerializeField] Animator anim;
    [SerializeField] MultiRotationConstraint rotationConstraint;

    public void Initialize(Brain brain)
    {
         _brain = brain; 
         IsReady = true;
    }
    void Update()
    {
        anim.SetLayerWeight(1, _brain.combat.IsAttacking ? 1 : 0);
        rotationConstraint.weight = _brain.combat.IsAttacking ? 1 : 0;
    }

    public void AE_Attack(int  num)
    {
       _brain.combat.AE_Attack(num);
    }
    
    public void MoveInput(float hor, float ver)
    {
        anim.SetFloat("moveHor", hor);
        anim.SetFloat("moveVer", ver);
    }

}
