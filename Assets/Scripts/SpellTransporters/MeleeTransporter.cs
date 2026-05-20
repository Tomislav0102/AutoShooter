using UnityEngine;

public class MeleeTransporter : SpellTransporter
{
    public override void InitializeMe(SpellMain spellMain)
    {
        spellMain.myTransform.position += spellMain.spell.areaOfEffect * 0.5f * spellMain.myTransform.forward;
        base.InitializeMe(spellMain);

    }
}
