using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Brain : EventBus
{
    public Faction faction;
    public Transform myTransform;
    public bool IsPlayer() => Ga.me.team[Faction.GoodGuys].Contains(myTransform);
    public Rigidbody myRigid;
    public SphereCollider myCollider;
    [SerializeField, Range(1, 5)] int size = 1;
    [SerializeField] Transform parPs;
    [Title("Body")]
    public GenOrder skin;
    [SerializeField] GameObject characterGo, healthGo, statusGo, locoGo, combatGo;
    [HideInInspector] public Character myChar;
    [HideInInspector] public Health health;
    [HideInInspector] public Status status;
    [HideInInspector] public Loco loco;
    [HideInInspector] public Combat combat;
    System.Action<Disposition> _onDispoChange;
    System.Action _onHitTaken;
    
    void Awake()
    {
        if (characterGo.TryGetComponent(out Character c))
        {
            myChar = c;
            myChar.Br = this;
        }
        if (healthGo.TryGetComponent(out Health h))
        {
            health = h;
            health.Br = this;
        }
        if (statusGo.TryGetComponent(out Status s))
        {
            status = s;
            status.Br = this;
        }
        if (locoGo.TryGetComponent(out Loco l))
        {
            loco = l;
            loco.Br = this;
        }
        if (combatGo.TryGetComponent(out Combat co))
        {
            combat = co;
            combat.Br = this;
        }
        ChangeFaction(faction);
        ChangeSize(size);
        ParticleSystem ps = Instantiate(Ga.me.psSpawn, myTransform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)), Ga.me.transform);
        ps.transform.localScale = size * Vector3.one;
        ps.Play();

    }

    [Title("Debug")] 
    public bool debugGeneral;
    [Button]
    void ChangeSize(int newSize)
    {
        size = newSize;
        myCollider.radius = size * 0.5f;
        parPs.localPosition = parPs.localPosition.y * size * Vector3.up;
        parPs.localScale = size * Vector3.one;
        
        if (loco == null) return;
        loco.transform.localScale = size * Vector3.one;
        E_Loco eLoco  = loco as E_Loco;
        if (eLoco != null) eLoco.agent.radius = size * 0.5f;
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
        
        for (int i = 0; i < System.Enum.GetNames(typeof(Faction)).Length; i++)
        {
           if (Ga.me.team[(Faction)i].Contains(myTransform)) Ga.me.team[(Faction)i].Remove(myTransform);
        }
        Ga.me.team[faction].Add(myTransform);

        
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
