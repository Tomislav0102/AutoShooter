using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpellGroup : MonoBehaviour
{
    protected SpellMain[] mySpells;
    public Transform myTransform;
    protected Brain owner;

    // gameObject has spells as children
    // spell is defined in inspector, caster cant change it
    public virtual void InitializeMe(Brain ownersBrain)
    {
        owner = ownersBrain;
        mySpells = Utils.AllChildren<SpellMain>(myTransform);
    }
    
    //gameObject has no children
    //methods below pass data fom caster to spell
    public virtual void InitializeMe(Brain ownersBrain, SpellMain[] spellsToAdd)
    {
        owner = ownersBrain;
        mySpells = spellsToAdd;
        for (int i = 0; i < mySpells.Length; i++)
        {
            mySpells[i].myTransform.parent = myTransform;
        }
    }
    public virtual void InitializeMe(Brain ownersBrain, SpellMain spellToInstantiate)
    {
        owner = ownersBrain;
        mySpells = new SpellMain[1];
        mySpells[0] = spellToInstantiate;
    }

}
