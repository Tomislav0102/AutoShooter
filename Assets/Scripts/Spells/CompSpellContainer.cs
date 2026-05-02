using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CompSpellContainer : MonoBehaviour
{
    public Transform myTransform;
    public CompSpell comp;
    [ReadOnly] public Brain ownersBrain;
    [ReadOnly] public ISpellTransporter iTransporter;
    [ReadOnly] public Spell spell;
    [ReadOnly] public bool isActive;

    public void InitializeMe(Brain brain, Dictionary<Element, float> damage = null)
    {
        ownersBrain = brain;
        iTransporter = GetComponentInChildren<ISpellTransporter>();
        spell = GetComponentInChildren<Spell>();
    }
    

}