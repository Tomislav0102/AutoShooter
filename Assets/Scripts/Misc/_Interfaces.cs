using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInit
{
    Brain Br { get; set; } 
    bool IsInitialized { get; set; }
}

public interface ITakeDamage : IInit
{
    void TakeDamage(InjectHealth dam);
};


public interface IObstacle { };
