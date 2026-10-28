using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Destructible : MonoBehaviour
{
    [SerializeField] Transform parMeshes;
    GameObject[] _meshes; 
    [SerializeField, Range(0f, 1f)] float spawnChance = 1f;

    void Awake()
    {
        parMeshes.Rotate(Vector3.up, Random.Range(0, 360));
        _meshes = Utils.AllChildrenGameObjects(parMeshes);
        Utils.ActivateOneArrayElement(_meshes, Random.Range(0, _meshes.Length));
    }

    void OnTriggerEnter(Collider other)
    {
      //  if (other != Ga.me.team.playersBrain.myCollider) return;
        if (Random.value < spawnChance)
        {
            Instantiate(Ga.me.dropPrefab, transform.position + Vector3.up, Quaternion.identity);
        }
        Instantiate(Ga.me.psWoodBreak, transform.position + Vector3.up, Quaternion.identity);
        Destroy(gameObject);
    }
}
