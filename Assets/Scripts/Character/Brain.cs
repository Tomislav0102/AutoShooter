using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Brain : MonoBehaviour
{
    public System.Action onAllChildrenInitialized; //redundant?
    [field:SerializeField] public Faction Faction { get; set; }
    public Transform myTransform;
    public SphereCollider myCollider;
    public NavMeshAgent agent;
    [SerializeField] Transform fakeShadow;
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
            character.onStatChange += (Stats stat, float val) =>
            {
                switch (stat)
                {
                    case Stats.BodySize:
                        ChangeSize(val * 0.01f);
                        break;
                }
            };
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
        ParticleSystem ps = Instantiate(Ga.me.psSpawn, myTransform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)), Ga.me.transform);
        ps.transform.localScale = 0.01f * character.GetStat(Stats.BodySize) * Vector3.one;
        ps.Play();
        onAllChildrenInitialized?.Invoke();
    }


    [Title("Debug")] 
    public bool debug;
    public bool canAttack = true;

   // [Button]
    void ChangeSize(float newSize)
    {
        myCollider.radius = newSize * 0.5f;
        parPs.localPosition = parPs.localPosition.y * newSize * Vector3.up;
        parPs.localScale = newSize * Vector3.one;
        fakeShadow.localScale = newSize * Vector3.one;
        if (loco == null) return;
        loco.transform.localScale = newSize * Vector3.one;
        agent.radius = newSize * 0.5f;
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
