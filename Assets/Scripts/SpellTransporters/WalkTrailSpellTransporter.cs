using System;
using System.Collections.Generic;
using UnityEngine;

public class WalkTrailSpellTransporter : MonoBehaviour, ISpellTransporter
{
   // [SerializeField] CompSpellContainer myComp;
    float _timer;
    const float CONST_SpawnRate = 0.1f;
    HashSet<Collider> _spawns;
    float _radius;
    int _counter;
    
    // public override void InitializeMe(Brain br, Spell[] spellPrefab, Dictionary<Element, float> damage)
    // {
    //     base.InitializeMe(br, spellPrefab, damage);
    //     _radius = spellPrefab[_counter].areaOfEffect * 0.5f;
    //     _spawns = new HashSet<Collider>();
    //     isActive = true;
    // }
    //
    //
    // void Update()
    // {
    //     if (!isActive) return;
    //     _timer += Time.deltaTime;
    //     if (_timer >= CONST_SpawnRate)
    //     {
    //         _timer = 0;
    //         Collider[] colliders = Physics.OverlapSphere(brain.myTransform.position, _radius * 2, Ga.me.gameData.laySpells);
    //         for (int i = 0; i < colliders.Length; i++)
    //         {
    //             if (_spawns.Contains(colliders[i])) return;
    //         }
    //
    //         Spell spell = Instantiate(spellPrefabToInstantiate[_counter], brain.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
    //         spell.InitializeMe(brain, dam);
    //         _spawns.Add(spell.comp.mySphereCollider);
    //         _counter = (1 + _counter) % spellPrefabToInstantiate.Length;
    //     }
    //     
    // }
}