using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using TomoJoystick;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-10)]
public class Ga : MonoBehaviour
{
    public static Ga me;
    [BoxGroup("Enemy spawns")]
    public Transform spawnArea;
    [BoxGroup("Enemy spawns")]
    [SerializeField] int numOfEnemies;
    [BoxGroup("Enemy spawns")]
    [SerializeField] E_Loco[] enemyPrefabs;
    [BoxGroup("Enemy spawns")]
    [SerializeField] Transform parWaypoints;
    [HideInInspector] public Transform[] waypoints;
    public SpellManager spells;
    public Transform barContainer;
    public RectTransform healthBarPrefab;
    public Transform floatingContainer;
    public FloatingText floatingTextPrefab;
    public Transform cameraRigTransform;
    [HideInInspector] public Camera cam;
    [HideInInspector] public Transform playerTransform;
    public Dictionary<Faction, HashSet<Transform>> team = new Dictionary<Faction, HashSet<Transform>>();
    public TomoJoystick.Joystick joystick;
    public LayerMask layEnemies, layPlayer, layTest;
    public SpecialUi specialUi;
    
    [Button]
    void SpawnEnemies()
    {
        for (int i = 0; i < numOfEnemies; i++)
        {
            SpawnEnemy();
        }
    }
    void Awake()
    {
        me = this;
        
        cam = cameraRigTransform.GetComponentInChildren<Camera>();
        waypoints = Utils.AllChildren<Transform>(parWaypoints);
        team = new Dictionary<Faction, HashSet<Transform>>()
        {
            { Faction.Ally, new HashSet<Transform>() },
            { Faction.Foe, new HashSet<Transform>() }
        };
    }

    void SpawnEnemy()
    {
        bool canSpawn = false;
        Vector3 spawnPoint = Vector3.zero;
        while (!canSpawn)
        {
            float x = spawnArea.position.x + Random.Range(-spawnArea.localScale.x * 0.5f, spawnArea.localScale.x * 0.5f);
            float z = spawnArea.position.z + Random.Range(-spawnArea.localScale.z * 0.5f, spawnArea.localScale.z * 0.5f);
            spawnPoint = new Vector3(x, 0f, z);
            Collider[] colliders = Physics.OverlapSphere(spawnPoint + Vector3.up, 1f);
            canSpawn = colliders.Length == 0;
        }
        
        Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPoint, Quaternion.identity);
    }

    void OnEnable()
    {
        EventBus.OnCharDeath += CallEv_OnCharDeath;
    }
    void OnDisable()
    {
        EventBus.OnCharDeath -= CallEv_OnCharDeath;
    }

    void CallEv_OnCharDeath(Transform tr)
    {
        if (team[Faction.Ally].Contains(tr))
        {
            team[Faction.Ally].Remove(tr);
            if (tr == playerTransform)
            {
                EventBus.OnPlayerDeath?.Invoke();
                print("Player is dead");
            }
            else
            {
                Destroy(tr.gameObject);
                print("Summon is dead");
            }
        }
        else if (team[Faction.Foe].Contains(tr))
        {
            team[Faction.Foe].Remove(tr);
            Destroy(tr.gameObject);
        }
    }

}
