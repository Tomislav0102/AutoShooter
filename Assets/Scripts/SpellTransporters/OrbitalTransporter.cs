using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class OrbitalTransporter : SpellTransporter
{
    Spell[] _spells;
    [ReadOnly] public float distanceFromAnchor;
    [ReadOnly] public Transform anchor;
    
    
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