using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WalkTrailGroup : SpellGroup
{
    float _timer;
    const float CONST_SpawnRate = 0.1f;
    HashSet<Collider> _spawns;
    float _radius;

    public override void InitializeMe(Brain ownersBrain)
    {
        base.InitializeMe(ownersBrain);
        _radius = spellInstantiated.spell.areaOfEffect * 0.5f;
        _spawns = new HashSet<Collider>();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= CONST_SpawnRate)
        {
            _timer = 0;
            Collider[] colliders = Physics.OverlapSphere(brain.myTransform.position, 
                _radius * 2, 
                Utils.MyLayer(Ga.me.gameData.laySpell));
            for (int i = 0; i < colliders.Length; i++)
            {
                if (_spawns.Contains(colliders[i])) return;
            }

            SpellMain spell = Instantiate(spellInstantiated, brain.myTransform.position, Quaternion.identity, myTransform);
            spell.InitializeMe(brain, myDamage, () =>
            {
                if (_spawns.Contains(spell.mySphereCollider)) _spawns.Remove(spell.mySphereCollider);
            });
            _spawns.Add(spell.mySphereCollider);
        }
        
    }
}
