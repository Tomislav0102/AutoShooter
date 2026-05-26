using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// area effect that last a certain time (DOT)
/// </summary>
public class S_Area : Spell
{
    float _timer = Mathf.Infinity;
    public override void InitializeMe(SpellMain mainSpell)
    {
        base.InitializeMe(mainSpell);
        spellParticles.InitializeMe(mainSpell);
    }
    
    protected override void Update()
    {
        base.Update();
        if (MyPhase != Phase.SpellRuns) return;
        
        _timer += Time.deltaTime;
        if (_timer < 1f) return;
        if (collidersDetected.Count == 0) return;
        
        _timer = 0f;
        foreach (Collider item in collidersDetected)
        {
            if (item == null) continue; //sometimes its null with afterEffect (Unity bug?)
            if (item.TryGetComponent(out Brain collBrain) && 
                Utils.CanTargetFaction(main.OwnersBrain.Faction, collBrain.Faction, myFactionTarget))
            {
                collBrain.health.TakeDamage(injectHealthData);
                main.onHitTarget?.Invoke(collBrain);
            }
        }
    }

    public override void OnTriggerEnterCallBack(Collider other)
    {
        base.OnTriggerEnterCallBack(other);
        collidersDetected.Add(other);
    }

    public override void OnTriggerExitCallBack(Collider other)
    {
        base.OnTriggerExitCallBack(other);
        collidersDetected.Remove(other);
    }

}
