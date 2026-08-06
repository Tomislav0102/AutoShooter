using System.Collections.Generic;
using UnityEngine;


public class EffectScreen : SpellEffect
{
    public override SpellMain Spell
    {
        get => base.Spell;
        set
        {
            base.Spell = value;
            value.mySphereCollider.enabled = false;
            value.myCapsuleCollider.enabled = false;

        }
    }

    public void Hit()
    {
        List<Transform> targets = Utils.AllOnScreen(Ga.me.team.ValidTargets(Spell.OwnersBrain.Faction));
        foreach (Transform item in targets)
        {
            Spell.HitGeneric<Transform>(item, out Brain br);
        }
    }
}
