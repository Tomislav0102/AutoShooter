using System.Collections.Generic;
using UnityEngine;


public class EffectScreen : MonoBehaviour, IIniSpell
{
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value;
            value.mySphereCollider.enabled = false;
            value.myCapsuleCollider.enabled = false;

        }
    }
    SpellMain _spell;
    
    public void Hit()
    {
        List<Brain> targets = Utils.AllOnScreen(Ga.me.team.ValidTargets(Spell.OwnersBrain.Faction));
        foreach (Brain item in targets)
        {
            Spell.HitGeneric<Brain>(item, out _);
        }
    }
}
