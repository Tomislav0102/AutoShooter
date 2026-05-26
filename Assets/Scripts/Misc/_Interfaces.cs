using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInit
{
    Brain Br { get; set; } 
    bool IsInitialized { get; set; }
}

public interface IFaction
{
    Faction Faction { get; set; }
}




