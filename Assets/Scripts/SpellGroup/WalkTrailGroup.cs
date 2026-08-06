using System.Collections.Generic;
using UnityEngine;

public class WalkTrailGroup : SpellGroup
{
    SpellMain _main;
    float _timer;
    const float CONST_SpawnRate = 0.1f;
    HashSet<Collider> _spawns;
    float _radius;

    public override void InitializeMe(Brain ownersBrain, SpellMain spellToInstantiate)
    {
        base.InitializeMe(ownersBrain, spellToInstantiate);
        _main = spellToInstantiate;
        _radius = _main.areaOfEffect * 0.5f;
        _spawns = new HashSet<Collider>();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= CONST_SpawnRate)
        {
            _timer = 0;
            Collider[] colliders = Physics.OverlapSphere(owner.myTransform.position, 
                _radius * 2, 
                Utils.MyLayer(Ga.me.gameData.laySpell));
            for (int i = 0; i < colliders.Length; i++)
            {
                if (_spawns.Contains(colliders[i])) return;
            }

            SpellMain spell = Instantiate(_main, owner.myTransform.position, Quaternion.identity, myTransform);
            spell.InitializeMe(owner, () =>
            {
                if (_spawns.Contains(spell.mySphereCollider)) _spawns.Remove(spell.mySphereCollider);
            });
            _spawns.Add(spell.mySphereCollider);
        }
        
    }
}
