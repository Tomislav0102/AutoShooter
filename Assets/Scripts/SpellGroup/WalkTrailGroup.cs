using System.Collections.Generic;
using UnityEngine;

public class WalkTrailGroup : SpellGroup
{
    float _timer;
    const float CONST_SpawnRate = 0.05f;
    HashSet<Collider> _spawns;
    float _radius;

    public override void InitializeMe(Brain ownersBrain)
    {
        base.InitializeMe(ownersBrain);
        for (int i = 0; i < mainPrefabPair.Length(); i++)
        {
            mainPrefabPair.GetValue(i).myBrain = ownersBrain;
        }
        _radius = groupPassData.setAreaOfEffect ? groupPassData.areaOfEffect : mainPrefabPair.GetKey(0).areaOfEffect;
        _spawns = new HashSet<Collider>();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < CONST_SpawnRate) return;

        _timer = 0;
        Collider[] colliders = Physics.OverlapSphere(owner.myTransform.position, _radius,
            Utils.MyLayer(Ga.me.gameData.laySpell));
        for (int i = 0; i < colliders.Length; i++)
        {
            if (_spawns.Contains(colliders[i])) return;
        }

        SpellMain spell = Instantiate(mainPrefabPair.GetKey(0), owner.myTransform.position, Quaternion.identity, myTransform);
        if (groupPassData.setAreaOfEffect) spell.areaOfEffect = groupPassData.areaOfEffect;
        if (groupPassData.setLifeTime) spell.lifeTime = groupPassData.lifeTime;
        spell.InitializeMe(owner, mainPrefabPair.GetValue(0), () =>
        {
            if (_spawns.Contains(spell.mySphereCollider)) _spawns.Remove(spell.mySphereCollider);
        });
        _spawns.Add(spell.mySphereCollider);

    }
}
