using System;
using System.Collections.Generic;
using UnityEngine;

public class S_A_Overlap : S_A
{

    protected override void Hit()
    {
        bool eventCall = false;
        Collider[] colliders = Physics.OverlapSphere(main.myTransform.position,
            areaOfEffect * 0.5f,
            Utils.MyLayers(new string[] {Ga.me.gameData.layActors, Ga.me.gameData.laySpell}));
        foreach (Collider item in colliders)
        {
            HitMethod(item, out Brain targetBrain);
            if (targetBrain != null)
            {
                main.onHitTarget?.Invoke(targetBrain);
                eventCall = true;
            }
        }
        if (!eventCall) main.onHitTarget?.Invoke(null);
        spellParticles.InitializeMe(main);
    }

}

