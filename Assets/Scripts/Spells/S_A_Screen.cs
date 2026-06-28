using System.Collections.Generic;
using UnityEngine;

public class S_A_Screen : S_A
{
    protected override void Hit()
    {
        base.Hit();
        List<Transform> targets = Utils.AllOnScreen(Ga.me.team.ValidTargets(main.OwnersBrain.Faction));
        foreach (Transform item in targets)
        {
            HitGeneric<Transform>(item, out Brain br);
        }
    }
}
