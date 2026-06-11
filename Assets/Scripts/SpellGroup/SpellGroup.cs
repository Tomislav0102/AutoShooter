using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpellGroup : MonoBehaviour
{
    protected SpellMain[] mySpells;
    [SerializeField] protected SpellMain spellInstantiated; //temp, will change after pool implementation
    public Transform myTransform;
    protected Brain brain;
    public Dictionary<Element, float> myDamage;

    public virtual void InitializeMe(Brain ownersBrain)
    {
        brain = ownersBrain;
        mySpells = Utils.AllChildren<SpellMain>(myTransform);
    }

}
