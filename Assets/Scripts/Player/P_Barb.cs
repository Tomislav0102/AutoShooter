using UnityEngine;

/// <summary>
/// This character is on hold because I can't find good attack animation.
/// Current one can't be used with animation events
/// </summary>
public class P_Barb : PlayerCombat
{
    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        print(num);
    }

}
