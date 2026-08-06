using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIniBrain 
{
    Brain Br { get; set; } 
}
public interface ITargetTracker
{
    Transform MyTarget { get; set; }
}
public interface IIniSpell
{
    SpellMain Spell { get; set; }
}



