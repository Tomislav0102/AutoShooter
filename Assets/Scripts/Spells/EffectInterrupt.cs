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
        if (!other.TryGetComponent(out SpellMain targetSpell)) return;
        if (targetSpell.OwnersBrain == Spell.OwnersBrain) return; //dont interrupt your own spells
        Spell.collidersDetected.Add(other);
        bool spellFound = false;
        for (int i = 0; i < spellsToInterrupt.Length; i++)
        {
            if (targetSpell.specialty != spellsToInterrupt[i]) continue;
            spellFound = true;
            break;
        }
        if (!spellFound) return;
        Spell.onHitTarget?.Invoke(targetSpell.OwnersBrain);
        switch (effect)
        {
            case SpellMain.HitEffectOnSpell.Nullify:
                targetSpell.MyPhase = SpellMain.Phase.EndEnd;
                break;
            case SpellMain.HitEffectOnSpell.Reflect:
                switch (targetSpell.specialty)
                {
                    case SpellMain.Specialty.General:
                        break;
                    case SpellMain.Specialty.Melee: //will this ever be used?
                        targetSpell.reflexCount--;
                        targetSpell.OwnersBrain = Spell.OwnersBrain;
                        targetSpell.HitGeneric(Spell.OwnersBrain, out _);
                        break;
                    case SpellMain.Specialty.Projectile:
                        targetSpell.reflexCount--;
                        targetSpell.transporter.ReflectProjectile(Spell.OwnersBrain);
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
