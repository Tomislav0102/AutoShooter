using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class S_Shield : Spell
{
    [Title("Shield")]
    [InfoBox("Only type matters. All instances of same type are treated the same. For example, any 'S_Bullet' in array detects all variations. If 'Spell' is in array that detects all.")]
    [SerializeField] Spell[] spellsToAffect;


    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Spell spell) && Utils.TargetFaction(myFaction) == spell.myFaction)
        {
            for (int i = 0; i < spellsToAffect.Length; i++)
            {
                if (spell.GetType() == spellsToAffect[i].GetType())
                {
                    spell.OnEnd();
                    return;
                }
            }
        }
    }
}
