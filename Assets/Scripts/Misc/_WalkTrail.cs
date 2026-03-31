using System.Collections.Generic;
using UnityEngine;

public class WalkTrail
{
    Brain _br;
    float _timer;
    float _pauseBetweenSpawns = 0.1f;
    HashSet<Collider> _spawns;
    GameObject _spellPrefab;
    float _radius;
    public bool isActive;

    /// <summary>
    /// Constructor should be used for resetting too (because Hashset keeps on growing with every spawn)
    /// </summary>
    /// <param name="br"></param>
    /// <param name="spellPrefab"></param>
    public WalkTrail(Brain br, GameObject spellPrefab)
    {
        _br = br;
        _spellPrefab = spellPrefab;
        _radius = spellPrefab.GetComponent<Spell>().radius;
        _spawns = new HashSet<Collider>();
        isActive = true;
    }
    
    public void UpdateLoop()
    {
        if (!isActive) return;
        _timer += Time.deltaTime;
        if (_timer >= _pauseBetweenSpawns)
        {
            _timer = 0;
            Collider[] colliders = Physics.OverlapSphere(_br.myTransform.position, _radius * 2);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (_spawns.Contains(colliders[i])) return;
            }

            Spell spell = GameObject.Instantiate(_spellPrefab, _br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform).GetComponent<Spell>();
            spell.InitializeMe(_br);
            _spawns.Add(spell.myCollider);
        }

    }
}