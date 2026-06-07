using System;
using System.Collections.Generic;
using UnityEngine;

public class S_Overlap : Spell
{
    float _timer = Mathf.Infinity;
    bool _oneHit;
    Brain _anchorBrain;

    public override void InitializeMe(SpellMain mainSpell)
    {
        base.InitializeMe(mainSpell);
        if (followTarget == null || followTarget.GetComponent<Brain>() == null) MyPhase = Phase.EndStart;
        _anchorBrain = followTarget.GetComponent<Brain>();
    }


    protected override void Update()
    {
        base.Update();
        if (!main.IsActive) return;
        if (MyPhase != Phase.SpellRuns) return;
        if (_oneHit) return;

        if (rateOfFire == 0)
        {
            Hit();
            _oneHit = true;
            return;
        }
        
        _timer += Time.deltaTime;
        if (_timer > rateOfFire)
        {
            _timer = 0f;
            Hit();
        }
    }

    void Hit()
    {
        if (_anchorBrain == null)
        {
            bool eventCall = false;
            Collider[] colliders = Physics.OverlapSphere(main.myTransform.position,
                areaOfEffect * 0.5f,
                Utils.MyLayer(Ga.me.gameData.layActors));
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
        else
        {
            HitMethod(_anchorBrain.myCollider, out _);
            main.onHitTarget?.Invoke(_anchorBrain);
        }
        
    }

}

