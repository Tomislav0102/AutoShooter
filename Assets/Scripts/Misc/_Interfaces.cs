using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInit
{
    Brain Br { get; set; } //instead of InitializeMe(Brain brain)
    bool IsReady { get; set; }
}

public interface ITakeDamage : IInit
{
    void TakeDamage(DamageData dam);
};

public interface IFaction : IInit
{
    Faction MyFaction { get; set; }
    Faction TargetFaction { get; set; }
}


public interface IObstacle { };
