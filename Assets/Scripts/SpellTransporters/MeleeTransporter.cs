using UnityEngine;

public class MeleeTransporter : SpellTransporter
{
    public override void InitializeMe(SpellControl spellControl, System.Action onAfterEffect = null)
    {
        base.InitializeMe(spellControl, onAfterEffect);
        main.myTransform.position += main.spell.areaOfEffect * 0.5f * main.myTransform.forward;
        main.spell.InitializeMe(main);
    }
}
