using UnityEngine;

public class S_SimpleTrigger : Spell
{
    public override void InitializeMe(SpellControl mainSpell)
    {
        base.InitializeMe(mainSpell);
        spellParticles.InitializeMe(areaOfEffect);
    }

    protected override void CallEv_OnTriggerEnter(Collider other)
    {
        base.CallEv_OnTriggerEnter(other);
        if (injectHealthData.damage.Count > 0 && 
            other.TryGetComponent(out ITakeDamage takeDamage) && 
            Utils.CanTargetFaction(myFaction, takeDamage.Br.faction, myFactionTarget))
        {
            takeDamage.TakeDamage(injectHealthData);
        }
        main.onEnd?.Invoke();
    }
}
