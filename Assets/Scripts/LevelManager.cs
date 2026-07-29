using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [BoxGroup("Enemy spawns")]
    public Transform spawnArea;
    [BoxGroup("Enemy spawns")]
    [SerializeField] int numOfEnemies;
    [BoxGroup("Enemy spawns")]
    [SerializeField] Brain[] enemyPrefabs;
    
    public bool InsideLevel(Vector3 myPos)
    {
        if (myPos.y < 0.2f)  return false;
        Vector2 lowerLeft = new Vector2(spawnArea.position.x - spawnArea.localScale.x * 0.5f, spawnArea.position.z - spawnArea.localScale.z * 0.5f);
        Vector2 upperRight = new Vector2(spawnArea.position.x + spawnArea.localScale.x * 0.5f, spawnArea.position.z + spawnArea.localScale.z * 0.5f);
        if (myPos.x <= lowerLeft.x || myPos.x >= upperRight.x ||
            myPos.z <= lowerLeft.y || myPos.z >= upperRight.y) return false;
        
        return true;
    }


    void Awake()
    {
        Ga.me.LevelMan = this;
    }

    void Start()
    {
        SceneManager.SetActiveScene(gameObject.scene);
        // if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        //
        // numOfEnemies = PlayerPrefs.GetInt(Ga.me.gameData.prefsTestEnemyCount);
        // SpawnEnemies();
    }


    [Button]
    public void SpawnEnemies()
    {
        StartCoroutine(SpawnDelay());
    }

    IEnumerator SpawnDelay()
    {
        for (int i = 0; i < numOfEnemies; i++)
        {
            spawnSingleEnemy();
            yield return new WaitForFixedUpdate();
        }
        
        void spawnSingleEnemy()
        {
            bool canSpawn = false;
            Vector3 spawnPoint = Vector3.zero;
            while (!canSpawn)
            {
                spawnPoint = Utils.GetRandomPosition(spawnArea);
                Collider[] colliders = Physics.OverlapSphere(spawnPoint, 1f, Utils.MyLayer(Ga.me.gameData.layActors));
                canSpawn = colliders.Length == 0;
            }
            
            Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPoint, Quaternion.identity);
        }
    }



}
