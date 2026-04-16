using System;
using UnityEngine;

/// <summary>
/// area effect, instantaneous
/// </summary>
public class S_HitSphere : Spell
{
    public override void InitializeMe(Brain brain)
    {
        base.InitializeMe(brain);
        Collider[] colliders = Physics.OverlapSphere(comp.myTransform.position, areaOfEffect * 0.5f);
        
        foreach (Collider item in colliders)
        {
            if (knockBack > 0 && item.TryGetComponent(out Brain br) && TarFaction() == br.faction)
            {
                if (br.loco != null)
                {
                    Vector3 dir = Utils.Direction(comp.myTransform.position, item.transform.position);
                    br.loco.KnockBack(dir, knockBack);
                }
            }
           // if (damData.damage > 0 && item.TryGetComponent(out ITakeDamage takeDamage) && TarFaction() == takeDamage.Br.faction)
            if (damData.damage > 0 && item.TryGetComponent(out ITakeDamage takeDamage))
            {
                if (brain.faction == Faction.Player)
                {
                    print(($"mine {brain.faction}"));
                    print($"target {TarFaction()}");
                    print($"take dam {takeDamage.Br.faction}");
                }
                takeDamage.TakeDamage(damData);
             //   print(item.name);

            }
        }

        if (comp.myMesh != null)
        {
            comp.myMesh.localScale = areaOfEffect * Vector3.one;
            comp.myMesh.GetComponentInChildren<ParticleSystem>().Play();
        }
        AfterEffect();
       // OnEnd();
    }
}
