using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class Brain : EventBus
{
    public Faction faction;
    public Transform myTransform;
    public Rigidbody myRigid;
    public SphereCollider myCollider;
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
            case Faction.Player:
                gameObject.layer = LayerMask.NameToLayer("Player");
                if (loco != null)
                {
                    Ga.me.team[Faction.Player].Add(myTransform);
                }
                break;
            case Faction.Monsters:
                gameObject.layer = LayerMask.NameToLayer("Enemy");
                if (loco != null)
                {
                    Ga.me.team[Faction.Monsters].Add(myTransform);
                }
                break;
            case Faction.Neutral:
                gameObject.layer = default;
                break;
        }
        if (myMaterials.Length >= (int)skin) myRenderer.material = myMaterials[(int)skin];
    }

    /// <summary>
    /// only for Dash
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other != myCollider && other.TryGetComponent(out Brain brain))
        {
            if (brain.loco != null) brain.loco.KnockBack(Utils.Direction(myTransform.position, other.transform
                .position), 5);
        }
    }

}
