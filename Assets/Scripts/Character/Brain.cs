using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class Brain : EventBus
{
    public Faction faction;
    public Rigidbody myRigid;
    [Title("References")]
    public Health health;
    public Loco loco;
    public Combat combat;

    public System.Action onHit;
    
    protected void Awake()
    {
        if (health != null) health.Br = this;
        if (loco != null) loco.Br = this;
        if (combat != null) combat.Br = this;
        switch (faction)
        {
            case Faction.Ally:
                gameObject.layer = LayerMask.NameToLayer("Player");
                if (loco != null)
                {
                    Ga.me.team[Faction.Ally].Add(loco.myTransform);
                }
                break;
            case Faction.Foe:
                gameObject.layer = LayerMask.NameToLayer("Enemy");
                if (loco != null)
                {
                    Ga.me.team[Faction.Foe].Add(loco.myTransform);
                }
                break;
            case Faction.Neutral:
                gameObject.layer = default;
                break;
        }
        onHit = () =>
        {
            if (loco != null) loco.Hit();
        };

    }
}
