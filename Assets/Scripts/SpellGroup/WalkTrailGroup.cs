using System.Collections.Generic;
using UnityEngine;

public class WalkTrailGroup : SpellGroup
{
    float _timer;
    const float CONST_SpawnRate = 0.1f;
    HashSet<Collider> _spawns;
    float _radius;

    public override void InitializeMe(Brain ownersBrain, MyDuo<SpellMain, PassData> duo)
    {
        base.InitializeMe(ownersBrain, duo);
        _radius = prefabsAndData.GetKey(0).areaOfEffect * 0.5f;
        _spawns = new HashSet<Collider>();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < CONST_SpawnRate) return;

        _timer = 0;
        Collider[] colliders = Physics.OverlapSphere(owner.myTransform.position,
            _radius * 2,
            Utils.MyLayer(Ga.me.gameData.laySpell));
        for (int i = 0; i < colliders.Length; i++)
        {
            if (_spawns.Contains(colliders[i])) return;
        }

        SpellMain spell = Instantiate(prefabsAndData.GetKey(0), owner.myTransform.position, Quaternion.identity, myTransform);
        spell.InitializeMe(owner, prefabsAndData.GetValue(0), () =>
        {
            if (_spawns.Contains(spell.mySphereCollider)) _spawns.Remove(spell.mySphereCollider);
        });
        _spawns.Add(spell.mySphereCollider);

    }
}
