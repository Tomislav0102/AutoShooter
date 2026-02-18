using System;
using UnityEngine;

public class PlayerHealth : Health
{

    protected override void Death()
    {
        base.Death();
        EventBus.OnAllyDeath?.Invoke(transform);
    }


}
