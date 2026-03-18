using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class Brain : EventBus
{
    public Faction faction;
    public Transform myTransform;
    public Rigidbody myRigid;
    [SerializeField] Renderer myRenderer;
    [SerializeField] Material[] myMaterials;
    [Title("Body")]
    public GenOrder skin;
    public Health health;
    public Loco loco;
    public Combat combat;
    
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
                    Ga.me.team[Faction.Ally].Add(myTransform);
                }
                break;
            case Faction.Foe:
                gameObject.layer = LayerMask.NameToLayer("Enemy");
                if (loco != null)
                {
                    Ga.me.team[Faction.Foe].Add(myTransform);
                }
                break;
            case Faction.Neutral:
                gameObject.layer = default;
                break;
        }
        if (myMaterials.Length >= (int)skin) myRenderer.material = myMaterials[(int)skin];
    }
}
