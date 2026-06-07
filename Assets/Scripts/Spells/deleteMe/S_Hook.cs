using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1 unit effect that last a certain time (DOT)
/// </summary>
public class S_Hook : Spell
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
        _anchorBrain.health.TakeDamage(injectHealthData);
        main.onHitTarget?.Invoke(_anchorBrain);

    }
}
