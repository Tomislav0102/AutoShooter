using System;
using System.Collections.Generic;
using UnityEngine;

public class WalkTrailSpellHelper : SpellHelper
{
    float _timer;
    float _pauseBetweenSpawns = 0.1f;
    HashSet<Collider> _spawns;
    float _radius;

    public override void InitializeMe(Brain br, Spell spellPrefab)
    {
        base.InitializeMe(br, spellPrefab);
        _radius = spellPrefab.radius;
        _spawns = new HashSet<Collider>();
        isActive = true;
    }


    void Update()
    {
        if (!isActive) return;
        _timer += Time.deltaTime;
        if (_timer >= _pauseBetweenSpawns)
        {
            _timer = 0;
            Collider[] colliders = Physics.OverlapSphere(brain.myTransform.position, _radius * 2);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (_spawns.Contains(colliders[i])) return;
            }

            Spell spell = Instantiate(spellPrefabToInstantiate, brain.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            spell.InitializeMe(brain);
            _spawns.Add(spell.myCollider);
        }
        
    }
}