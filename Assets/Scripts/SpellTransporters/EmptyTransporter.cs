using UnityEngine;

public class EmptyTransporter : SpellTransporter
{
    
    public override void InitializeMe(SpellControl spellControl)
    {
        base.InitializeMe(spellControl);
        main.spell.InitializeMe(spellControl);
    }
}
