using UnityEngine;

public class MeleeTransporter : SpellTransporter
{
    public override void InitializeMe(SpellControl spellControl)
    {
        base.InitializeMe(spellControl);
        main.myTransform.position += main.spell.areaOfEffect * 0.5f * main.myTransform.forward;
        main.spell.InitializeMe(main);
    }
}
