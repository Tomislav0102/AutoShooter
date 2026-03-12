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
    
    protected override void Awake()
    {
        base.Awake();
        if (health != null) health.Br = this;
        if (loco != null) loco.Br = this;
        if (combat != null) combat.Br = this;
        switch (faction)
        {
            case Faction.Ally:
                gameObject.layer = LayerMask.NameToLayer("Player");
                if (loco != null)
                {
                    GameManager.Instance.playersTeam.Add(loco.myTransform);
                }
                break;
            case Faction.Foe:
                gameObject.layer = LayerMask.NameToLayer("Enemy");
                if (loco != null)
                {
                    GameManager.Instance.allEnemies.Add(loco.myTransform);
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
