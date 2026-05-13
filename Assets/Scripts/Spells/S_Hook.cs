using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1 unit effect that last a certain time (DOT)
/// </summary>
public class S_Hook : Spell
{
    float _timer = float.MaxValue;
    ITakeDamage _anchorTakeDamage;

    public override void InitializeMe(SpellControl mainSpell)
    {
        base.InitializeMe(mainSpell);
        if (anchor == null || anchor.GetComponent<ITakeDamage>() == null) MyPhase = Phase.EndStart;
        _anchorTakeDamage = anchor.GetComponent<ITakeDamage>();
    }

    protected override void Update()
    {
        base.Update();
        _timer += Time.deltaTime;
        if (_timer > 1f)
        {
            _timer = 0f;
            _anchorTakeDamage.TakeDamage(injectHealthData);
            if (durationType == DurationType.Instant) MyPhase = Phase.EndStart;
        }
    }
}
