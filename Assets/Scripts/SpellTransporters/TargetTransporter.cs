using System;
using UnityEngine;

public class TargetTransporter : SpellTransporter
{
    public override SpellMain Spell
    {
        get => base.Spell;
        set
        {
            base.Spell = value;
        }
    }

    [SerializeField] bool followPosition = true;
    [SerializeField] bool followRotation;

    void Update()
    {
        switch (Spell.MyPhase)
        {
            case SpellMain.Phase.BeginWarning:
                break;
            case SpellMain.Phase.SpellRuns:
                if (target != null)
                {
                  if (followPosition)  Spell.myTransform.position = target.position;
                  if (followRotation)  Spell.myTransform.rotation = target.rotation;
                }
                else Spell.MyPhase = SpellMain.Phase.EndStart;
                break;
        }
    }
}
