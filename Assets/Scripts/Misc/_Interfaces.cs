using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitialization 
{
    Brain Br { get; set; } 
}
public interface ITargetTracker
{
    Transform MyTarget { get; set; }
}



