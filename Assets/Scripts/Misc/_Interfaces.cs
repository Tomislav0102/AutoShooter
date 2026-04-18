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
    void TakeDamage(InjectHealth dam);
};


public interface IObstacle { };
