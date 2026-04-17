using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
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
    [BoxGroup("Enemy spawns")]
    [SerializeField] Vector2 lowerLeft, upperRight;
    public bool InsideLevel(Vector3 myPos)
    {
        if (myPos.y < 0.2f) return false;
        if (myPos.x <= lowerLeft.x || myPos.x >= upperRight.x ||
            myPos.z <= lowerLeft.y || myPos.z >= upperRight.y) return false;
        return true;
    }
    public SoGameData gameData;
    public SoCharacter defCharacter;
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
    public LayerMask layEnemies, layPlayer;
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
            { Faction.Player, new HashSet<Transform>() },
            { Faction.Monsters, new HashSet<Transform>() }
        };
    }

    void OnEnable()
    {
        EventBus.OnCharDeath += CallEv_OnCharDeath;
    }
    void OnDisable()
    {
        EventBus.OnCharDeath -= CallEv_OnCharDeath;
    }

    void SpawnEnemy()
    {
        bool canSpawn = false;
        Vector3 spawnPoint = Vector3.zero;
        while (!canSpawn)
        {
            spawnPoint = Utils.GetRandomPosition(spawnArea);
            Collider[] colliders = Physics.OverlapSphere(spawnPoint, 1f);
            canSpawn = colliders.Length == 0;
        }
        
        Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPoint, Quaternion.identity);
    }


    void CallEv_OnCharDeath(Transform tr)
    {
        if (team[Faction.Player].Contains(tr))
        {
            team[Faction.Player].Remove(tr);
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
        else if (team[Faction.Monsters].Contains(tr))
        {
            team[Faction.Monsters].Remove(tr);
            Destroy(tr.gameObject);
        }
    }

}
