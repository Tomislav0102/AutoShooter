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
        List<Transform> targets = Utils.AllOnScreen(Ga.me.team.ValidTargets(Spell.OwnersBrain.Faction));
        foreach (Transform item in targets)
        {
            Spell.HitGeneric<Transform>(item, out _);
        }
    }
}
