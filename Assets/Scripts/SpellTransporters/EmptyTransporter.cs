using UnityEngine;

public class EmptyTransporter : SpellTransporter
{
    
    public override void InitializeMe(SpellControl spellControl, System.Action onAfterEffect = null)
    {
        base.InitializeMe(spellControl, onAfterEffect);
        main.spell.InitializeMe(main);
    }
}
