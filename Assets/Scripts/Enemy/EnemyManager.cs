using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] int startingCount;
    [SerializeField] Vector2 spawnHalfBounds;
    public HashSet<Transform> allEnemies = new HashSet<Transform>();

    [Button]
    void Test()
    {
        print(allEnemies.Count);
    }
    void Start()
    {
        //SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < startingCount; i++)
        {
            Vector2 randomSpawn = Vector2.zero;
            bool canSpawn = false;
            while (!canSpawn)
            {
                randomSpawn = new Vector2(Random.Range(-spawnHalfBounds.x, spawnHalfBounds.x), 
                    Random.Range(-spawnHalfBounds.y , spawnHalfBounds.y));
                RaycastHit2D hit = Physics2D.CircleCast(randomSpawn, 0.5f, Vector2.zero);
                canSpawn = hit.collider == null;
            }
            //  randomSpawn = i * Vector2.up; //debug
            Enemy en = Instantiate<Enemy>(enemyPrefab, randomSpawn, Quaternion.identity);
            en.name = $"Enemy {i}";
        }
    }

}
