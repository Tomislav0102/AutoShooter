using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// area effect, instantaneous
/// </summary>
public class S_OverlapSphere : Spell
{
    bool _oneHIt;

    protected override void Update()
    {
        base.Update();
        if (MyPhase != Phase.SpellRuns) return;
        if (_oneHIt) return;
        _oneHIt = true;
        
        Hit();
    }

    void Hit()
    {
        bool hasHit = false;
        Collider[] colliders = Physics.OverlapSphere(main.myTransform.position,
            areaOfEffect * 0.5f,
            Utils.MyLayer(Ga.me.gameData.layActors));
        foreach (Collider item in colliders)
        {
            HitMethod(item, out Brain targetBrain);
            if (targetBrain != null)
            {
                main.onHitTarget?.Invoke(targetBrain);
                hasHit = true;
            }
        }
        if (!hasHit) main.onHitTarget?.Invoke(null);
        spellParticles.InitializeMe(main);
        
    }

}

