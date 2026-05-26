using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1 unit effect that last a certain time (DOT)
/// </summary>
public class S_Hook : Spell
{
    float _timer = float.MaxValue;
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
        if (!main.spell.IsActive) return;
        _timer += Time.deltaTime;
        if (_timer > 1f)
        {
            _timer = 0f;
            _anchorBrain.health.TakeDamage(injectHealthData);
            main.onHitTarget?.Invoke(_anchorBrain);
        }
    }
}
