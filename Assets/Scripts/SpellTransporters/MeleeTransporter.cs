using UnityEngine;


public class MeleeTransporter : SpellTransporter
{
    public override SpellMain Spell
    {
        get => base.Spell;
        set
        {
            base.Spell = value;
            value.myTransform.position += value.range * 0.5f * value.myTransform.forward;
        }
    }
}
