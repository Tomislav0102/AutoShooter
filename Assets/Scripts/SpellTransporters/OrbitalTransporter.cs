using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class OrbitalTransporter : SpellTransporter
{
    [SerializeField] Transform tempParent;
    public int numOfSpells;
    Transform[] _spellTransforms;
    [ReadOnly] public Transform orbitingAnchor;

    public override void InitializeMe(SpellControl spellControl)
    {
        base.InitializeMe(spellControl);
        tempParent.parent = null;
        main.myTransform.parent = tempParent;
        _spellTransforms = new Transform[numOfSpells];
        float angle = 360f / numOfSpells;
        float distanceFromAnchor = main.spell.areaOfEffect + 1;
        for (int i = 0; i < numOfSpells; i++)
        {
            main.spell.InitializeMe(main);
            main.myTransform.rotation = Quaternion.Euler(0, angle * (i + 1), 0);
            main.myTransform.position += distanceFromAnchor * main.myTransform.forward;
            _spellTransforms[i] = main.myTransform;
        }
    }

    void Update()
    {
        tempParent.position = orbitingAnchor.position;
        tempParent.Rotate(20f * Time.deltaTime * Vector3.up);
        // main.myTransform.position = anchor.position;
       //  main.myTransform.Rotate(20f * Time.deltaTime * Vector3.up, Space.Self);
    }
    // public override void InitializeMe(Brain br, Spell[] spellPrefab, Dictionary<Element, float> damage) 
    // {
    //     base.InitializeMe(br, spellPrefab, damage);
    //     if (anchor == null) anchor = spellPrefab[0].anchor;
    //     if (anchor == null)
    //     {
    //         print("No anchor assigned, spell disabled.");
    //         return;
    //     }
    //     _spells = new Spell[spellPrefab.Length];
    //     float angle = 360f / spellPrefab.Length;
    //     for (int i = 0; i < spellPrefab.Length; i++)
    //     {
    //         _spells[i] = Instantiate(spellPrefabToInstantiate[i], myTransform.position, Quaternion.Euler(0, angle * (i + 1), 0), myTransform);
    //         _spells[i].comp.myTransform.Translate( distanceFromAnchor * Vector3.forward, Space.Self);
    //         _spells[i].InitializeMe(br, damage);
    //     }
    //     
    //     isActive = true;
    // }
    //
    // void Update()
    // {
    //     if (!isActive) return;
    //     myTransform.position = anchor.position;
    //     myTransform.Rotate(20f * Time.deltaTime * Vector3.up);
    // }
}