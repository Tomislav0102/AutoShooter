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
    [SerializeField] SpellMain.ReflexBehaviour[] spellsToInterrupt;
    
    
    public void OnTriggerEnterCallBack(Collider other)
    {
       if (!other.TryGetComponent(out SpellMain spell)) return;
       bool spellFound = false;
       for (int i = 0; i < spellsToInterrupt.Length; i++)
       {
           if (spell.reflexBehaviour != spellsToInterrupt[i]) continue;
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
               break;
       }
    }


}
