using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Brain : MonoBehaviour
{
    [field:SerializeField] public Faction Faction { get; set; }
    public Transform myTransform;
    public SphereCollider myCollider;
    public NavMeshAgent agent;
    [SerializeField] Transform fakeShadow;
    [Range(0.2f, 10f)] public float size = 1;
    [SerializeField] Transform parPs;
    Trap _trap;
    [Title("Body")]
    public GenOrder skin;
    [SerializeField] GameObject characterGo, healthGo, locoGo, combatGo;
    [HideInInspector] public Character character;
    [HideInInspector] public Skills skills;
    [HideInInspector] public SharedSkills sharedSkills;
    [HideInInspector] public Health health;
    [HideInInspector] public Status status;
    [HideInInspector] public Loco loco;
    [HideInInspector] public Combat combat;
    
    void Awake()
    {
        _trap = GetComponent<Trap>();
        if (_trap != null)
        {
            _trap.Br = this;
            return;
        }
        
        if (characterGo.TryGetComponent(out Character c))
        {
            character = c;
            character.Br = this;
        }
        if (characterGo.TryGetComponent(out Status s))
        {
            status = s;
            status.Br = this;
        }
        if (characterGo.TryGetComponent(out Skills sk))
        {
            skills = sk;
            skills.Br = this;
        }
        if (characterGo.TryGetComponent(out SharedSkills sharedSk))
        {
            sharedSkills = sharedSk;
            sharedSkills.Br = this;
        }
        if (healthGo.TryGetComponent(out Health h))
        {
            health = h;
            health.Br = this;
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
        Ga.me.team.JoinTeam(Faction, this);
        if (this != Ga.me.team.playersBrain) size *= Random.Range(0.9f, 1.1f);
        ChangeSize(size);
        ParticleSystem ps = Instantiate(Ga.me.psSpawn, myTransform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)), Ga.me.transform);
        ps.transform.localScale = size * Vector3.one;
        ps.Play();

    }


    [Title("Debug")] 
    public bool debug;
    public bool canAttack = true;

    [Button]
    void ChangeSize(float newSize)
    {
        size = newSize;
        myCollider.radius = size * 0.5f;
        parPs.localPosition = parPs.localPosition.y * size * Vector3.up;
        parPs.localScale = size * Vector3.one;
        fakeShadow.localScale = size * Vector3.one;
        if (loco == null) return;
        loco.transform.localScale = size * Vector3.one;
        agent.radius = size * 0.5f;
    }
    
    // [Button]
    // public void ToggleFaction()
    // {
    //     int f = (int)Faction;
    //     f = (1 + f) % 2;
    //     Faction = (Faction)f;
    //     Ga.me.team.ChangeTeam(Faction, this, GenChange.Add);
    // }
}
