using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using TomoJoystick;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class GameManager : EventBus
{
    public static GameManager Instance;
    [BoxGroup("Enemy spawns")]
    public Transform spawnArea;
    [BoxGroup("Enemy spawns")]
    [SerializeField] int numOfEnemies;
    [BoxGroup("Enemy spawns")]
    [SerializeField] E_Loco[] enemyPrefabs;
    [BoxGroup("Enemy spawns")]
    [SerializeField] Transform parWaypoints;
    [HideInInspector] public Transform[] waypoints;
    public Transform barContainer;
    public RectTransform healthBarPrefab;
    public Transform floatingContainer;
    public FloatingText floatingTextPrefab;
    public Transform cameraRigTransform;
    [HideInInspector] public Camera cam;
    const float CONST_CamEdgeBottom = 7f;
    [ReadOnly] public P_Loco loco;
    [HideInInspector] public Transform playerTransform;
    public HashSet<Transform> playersTeam = new HashSet<Transform>();
    public HashSet<Transform> allEnemies = new HashSet<Transform>();
    public TomoJoystick.Joystick joystick;
    public Projectile projectilePrefab;
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
    protected override void Awake()
    {
        Instance = this;
        base.Awake();
        
        cam = cameraRigTransform.GetComponentInChildren<Camera>();
        if (loco == null) loco = GameObject.FindAnyObjectByType<P_Loco>();
        playerTransform = loco.transform;
        playersTeam.Add(playerTransform);
        waypoints = Utils.AllChildren<Transform>(parWaypoints);
    }

    void FixedUpdate()
    {
        Vector3 targetPos = Vector3.Lerp(cameraRigTransform.position, playerTransform.position, 0.2f);
       // targetPos.z = Mathf.Max(targetPos.z, CONST_CamEdgeBottom);
        cameraRigTransform.position = targetPos;
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
    protected override void CallEv_OnCharDeath(Transform tr)
    {
        base.CallEv_OnCharDeath(tr);
        if (playersTeam.Contains(tr))
        {
            playersTeam.Remove(tr);
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
        else if (allEnemies.Contains(tr))
        {
            allEnemies.Remove(tr);
            Destroy(tr.gameObject);
        }
    }

}
