using UnityEngine;

public class Smb_InStateCallback : StateMachineBehaviour
{
    int _isAttacking = Animator.StringToHash("isAttacking");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(_isAttacking, true);
    }

    

}
