using System.Collections.Generic;
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
    public Character myChar;
    public GenOrder skin;
    public Health health;
    public Loco loco;
    public Combat combat;
    System.Action<Disposition> _onDispoChange;
    System.Action _onHitTaken;
    
    void Awake()
    {
        if (myChar != null) myChar.Br = this;
        if (health != null) health.Br = this;
        if (loco != null) loco.Br = this;
        if (combat != null) combat.Br = this;
        ChangeFaction(faction);
    }

    [Button]
    public void ToggleFaction()
    {
        ChangeFaction(Utils.TargetFaction(faction));
    }
    void ChangeFaction(Faction newFaction)
    {
        if (loco != null)
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(Faction)).Length; i++)
            {
               if (Ga.me.team[(Faction)i].Contains(myTransform)) Ga.me.team[(Faction)i].Remove(myTransform);
            }
        }
        switch (newFaction)
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

        if (myMaterials.Length >= (int)newFaction) myRenderer.material = myMaterials[(int)newFaction];
        
    }

    /// <summary>
    /// only for Dash
    /// </summary>
    /// <param name="other"></param>
    // void OnTriggerEnter(Collider other)
    // {
    //     if (other != myCollider && other.TryGetComponent(out Brain brain))
    //     {
    //         if (brain.loco != null) brain.loco.KnockBack(Utils.Direction(myTransform.position, other.transform
    //             .position), 5);
    //     }
    // }

}
