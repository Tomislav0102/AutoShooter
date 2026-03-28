using UnityEngine;

public class SMB_KnightSlashFx : StateMachineBehaviour
{
    [SerializeField] int index;
    P_Knight Knight(Animator anim)
    {
        if (_knight == null)
        {
            _knight = anim.GetComponent<Loco>().Br.combat as P_Knight;
        }
        return _knight;
    }
    P_Knight _knight;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Knight(animator).SlashFx(index);
    }

}
