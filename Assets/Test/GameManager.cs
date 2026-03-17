using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

namespace TestApp
{
    [DefaultExecutionOrder(-10)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public TomoJoystick.Joystick joystick;
        public Transform plTransform;
        [HideInInspector] public Transform camRigTransform;
        public Camera cam;
        [Title("Spawner")]
        public bool spawnEnemies;
        [ShowIf(nameof(spawnEnemies))]
        public int numEnemies;
        [ShowIf(nameof(spawnEnemies))]
        public GameObject enPrefab;
        [ShowIf(nameof(spawnEnemies))]
        public Transform parEnemies;
        [ShowIf(nameof(spawnEnemies))]
        public HashSet<Transform> enemies = new HashSet<Transform>();
        public bool spawnObstacles;
        [ShowIf(nameof(spawnObstacles))]
        public int numObstacles;
        [ShowIf(nameof(spawnObstacles))]
        public GameObject obstaclePrefab;
        [ShowIf(nameof(spawnObstacles))]
        public Transform obstacleParent;
        
        void Awake()
        {
            Instance = this;
            camRigTransform = cam.transform.parent;   
        }

        IEnumerator Start()
        {
            if (spawnObstacles) SpawnObstacles();
            yield return new WaitForSeconds(0.1f);
            if (spawnEnemies) SpawnEnemies();
        }

        public void ButtonQuit()
        {
            SceneManager.LoadScene("MainMenu");
        }
        public void ButtonReset()
        {
            SceneManager.LoadScene(gameObject.scene.name);
        }
        public void ButtonDestroyAllEnemies()
        {
            foreach (Transform item in enemies)
            {
                Destroy(item.gameObject);
            }
            enemies.Clear();
        }

        [Button]
        void SpawnEnemies()
        {
            numEnemies = PlayerPrefs.GetInt("ens");
            for (int i = 0; i < numEnemies; i++)
            {
                Instantiate(enPrefab, Utils.MakeV3(numEnemies * 0.1f * Random.insideUnitCircle) + parEnemies.position, Quaternion.identity, parEnemies);
            }
        }

        
        void SpawnObstacles()
        {
            float width = 30;
            float height  = 30;
            for (int i = 0; i < numObstacles; i++)
            {
                float x = Random.Range(-width, width);
                float z = Random.Range(-height, height);
                GameObject go = Instantiate(obstaclePrefab, new Vector3(x, 0, z), Quaternion.identity, obstacleParent);
                go.transform.localScale = new Vector3(1, 1.7f, Random.Range(1f, 10f));
                go.transform.Rotate(Random.Range(0, 360) * Vector3.up);
            }
        }

        
    }
}
