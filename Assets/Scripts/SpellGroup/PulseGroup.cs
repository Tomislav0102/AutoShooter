using System;
using UnityEngine;

public class PulseGroup : SpellGroup
{
    
    public float rof;
    float _timer;
    
    public override void InitializeMe(Brain ownersBrain)
    {
        base.InitializeMe(ownersBrain);
        
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < rof) return;
        _timer = 0;

        SpellMain spellMain = Instantiate(spellInstantiated, brain.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        spellMain.InitializeMe(brain);
    }
}
