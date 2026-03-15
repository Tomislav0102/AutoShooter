using System.Collections;
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


public interface IObstacle { };
