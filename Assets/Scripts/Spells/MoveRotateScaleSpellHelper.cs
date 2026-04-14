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
            this.enabled = false;
            return;
        }
        _spells = new Spell[numOfSpells];
        float angle = 360f / numOfSpells;
        for (int i = 0; i < numOfSpells; i++)
        {
            _spells[i] = Instantiate(spellPrefabToInstantiate, myTransform.position, Quaternion.Euler(0, angle * (i + 1), 0), myTransform);
            _spells[i].comp.myTransform.Translate( distanceFromTarget * Vector3.forward, Space.Self);
        }
        
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;
        myTransform.position = anchor.position;
        myTransform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
    }
}