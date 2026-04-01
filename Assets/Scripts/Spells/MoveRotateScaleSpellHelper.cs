using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MoveRotateScaleSpellHelper : SpellHelper
{
    [Title("Rotation around target")]
    [SerializeField] int numOfSpells;
    [SerializeField] float rotationSpeed, distanceFromTarget;
    Spell[] _spells;
    public Transform anchor;
    
    public override void InitializeMe(Brain br, Spell spellPrefab)
    {
        base.InitializeMe(br, spellPrefab);
        if (anchor == null) anchor = spellPrefab.anchor;
        if (anchor == null)
        {
            print("No anchor assigned, spell disabled.");
            return;
        }
        _spells = new Spell[numOfSpells];
        float angle = 360f / numOfSpells;
        for (int i = 0; i < numOfSpells; i++)
        {
            _spells[i] = Instantiate(spellPrefabToInstantiate, anchor.position, Quaternion.Euler(0, angle * (i + 1), 0), brain.myTransform);
            _spells[i].myTransform.Translate( distanceFromTarget * Vector3.forward, Space.Self);
        }
        
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;
        for (int i = 0; i < numOfSpells; i++)
        {
            _spells[i].myTransform.RotateAround(anchor.position, Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}