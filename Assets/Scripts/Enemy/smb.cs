using UnityEngine;

public class smb : StateMachineBehaviour
{
    int _isAttacking = Animator.StringToHash("isAttacking");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(_isAttacking, true);
    }


}
