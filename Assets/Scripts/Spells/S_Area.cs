using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// area effect that last a certain time (DOT)
/// </summary>
public class S_Area : Spell
{

    float _timer = Mathf.Infinity;
    public override void InitializeMe(SpellControl mainSpell)
    {
        base.InitializeMe(mainSpell);
        spellParticles.InitializeMe(areaOfEffect);
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
            if (item.TryGetComponent(out ITakeDamage takeDamage) && Utils.CanTargetFaction(myFaction, takeDamage.Br.faction, myFactionTarget))
            {
                takeDamage.TakeDamage(injectHealthData);
            }
        }
    }

    protected override void CallEv_OnTriggerEnter(Collider other)
    {
        base.CallEv_OnTriggerEnter(other);
        if (lifeTime == 0) return;
        collidersDetected.Add(other);
    }

    protected override void CallEv_OnTriggerExit(Collider other)
    {
        base.CallEv_OnTriggerExit(other);
        if (lifeTime == 0) return;
        collidersDetected.Remove(other);
    }

}
