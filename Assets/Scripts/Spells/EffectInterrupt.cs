using System;
using UnityEngine;

public class EffectInterrupt : MonoBehaviour, IIniSpell
{
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value;
        }
    }
    SpellMain _spell;

    [SerializeField] SpellMain.HitEffectOnSpell effect;
    [SerializeField] SpellMain.Specialty[] spellsToInterrupt;


    public void OnTriggerEnterCallBack(Collider other)
    {
        if (Spell.collidersDetected.Contains(other)) return;
        if (!other.TryGetComponent(out SpellMain spell)) return;
        if (spell.OwnersBrain == Spell.OwnersBrain) return; //dont interrupt your own spells
        Spell.collidersDetected.Add(other);
        bool spellFound = false;
        for (int i = 0; i < spellsToInterrupt.Length; i++)
        {
            if (spell.specialty != spellsToInterrupt[i]) continue;
            spellFound = true;
            break;
        }
        if (!spellFound) return;

        switch (effect)
        {
            case SpellMain.HitEffectOnSpell.Nullify:
                spell.MyPhase = SpellMain.Phase.EndEnd;
                break;
            case SpellMain.HitEffectOnSpell.Reflect:
                switch (spell.specialty)
                {
                    case SpellMain.Specialty.General:
                        break;
                    case SpellMain.Specialty.Melee: //will this ever be used?
                        spell.reflexCount--;
                        spell.OwnersBrain = Spell.OwnersBrain;
                        spell.HitGeneric(Spell.OwnersBrain, out _);
                        break;
                    case SpellMain.Specialty.Projectile:
                        spell.reflexCount--;
                        spell.transporter.ReflectProjectile(Spell.OwnersBrain);
                        break;
                }
                break;
        }
    }

    public void OnTriggerExitCallBack(Collider other)
    {
        if (!Spell.collidersDetected.Contains(other)) return;
        Spell.collidersDetected.Remove(other);
    }

    


}
