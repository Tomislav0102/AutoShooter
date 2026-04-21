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
    public Status status;
    public Loco loco;
    public Combat combat;
    System.Action<Disposition> _onDispoChange;
    System.Action _onHitTaken;
    
    void Awake()
    {
        if (myChar != null) myChar.Br = this;
        if (health != null) health.Br = this;
        if (status != null) status.Br = this;
        if (loco != null) loco.Br = this;
        if (combat != null) combat.Br = this;
        ChangeFaction(faction);
        ParticleSystem ps = Instantiate(Ga.me.psSpawn, myTransform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
        ps.Play();

    }

    [Button]
    public void ToggleFaction()
    {
        int f = (int)faction;
        f = (1 + f) % 2;
        ChangeFaction((Faction)f);
    }
    void ChangeFaction(Faction newFaction)
    {
        faction = newFaction;
        
        if (loco != null)
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(Faction)).Length; i++)
            {
               if (Ga.me.team[(Faction)i].Contains(myTransform)) Ga.me.team[(Faction)i].Remove(myTransform);
            }
            Ga.me.team[faction].Add(myTransform);
        }

        if (myMaterials.Length >= (int)faction) myRenderer.material = myMaterials[(int)faction];
        
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
