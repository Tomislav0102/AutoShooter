using System;
using UnityEngine;

public class SummonHealth : Health
{
    protected override void Awake()
    {
        base.Awake();
        gm.playersTeam.Add(transform);
    }

    protected override void Death()
    {
        base.Death();
        EventBus.OnAllyDeath?.Invoke(transform);
    }
}
