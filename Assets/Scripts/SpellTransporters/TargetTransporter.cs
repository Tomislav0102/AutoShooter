using System;
using UnityEngine;

public class TargetTransporter : SpellTransporter
{
    [SerializeField] bool followPosition = true;
    [SerializeField] bool followRotation;

    void Update()
    {
        switch (main.spell.MyPhase)
        {
            case Spell.Phase.BeginWarning:
                break;
            case Spell.Phase.SpellRuns:
                if (target != null)
                {
                  if (followPosition)  main.myTransform.position = target.position;
                  if (followRotation)  main.myTransform.rotation = target.rotation;
                }
                else main.spell.MyPhase = Spell.Phase.EndStart;
                break;
            case Spell.Phase.EndStart:
                break;
            case Spell.Phase.EndEnd:
                break;
        }
    }
}
