using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class Brain : EventBus
{
    [field:SerializeField] public Faction Faction { get; set; }
    public Transform myTransform;
    public Rigidbody myRigid;
    public SphereCollider myCollider;
    [SerializeField] Transform fakeShadow;
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
        Ga.me.team.ChangeTeam(Faction, this, GenChange.Add);
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
        fakeShadow.localScale = size * Vector3.one;
        if (loco == null) return;
        loco.transform.localScale = size * Vector3.one;
        E_Loco eLoco  = loco as E_Loco;
        if (eLoco != null) eLoco.agent.radius = size * 0.5f;
    }
    [Button]
    public void ToggleFaction()
    {
        int f = (int)Faction;
        f = (1 + f) % 2;
        Faction = (Faction)f;
        Ga.me.team.ChangeTeam(Faction, this, GenChange.Add);
    }
}
