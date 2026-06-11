using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class Test : SerializedMonoBehaviour
{
    public Material[] mats;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float radius;
    [Title("Generate")]
    public List<GameObject> gos;
    public Transform parent;
    public GameObject prefab;
    public int gridSize;
    public float offset;
    float _size = 1f;


    void Update()
    {
        ResetColor();
        Collider[] cols = Physics.OverlapCapsule(startPosition, endPosition, radius);
        foreach (Collider item in cols)
        {
            item.GetComponent<MeshRenderer>().material = mats[1];
        }
    }

    [Button]
    void Generate()
    {
        while (parent.childCount > 0)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
        }
        parent.transform.position = Vector3.zero;
        gos = new List<GameObject>();
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                gos.Add(Instantiate(prefab, new Vector3(i + (offset * i), 0f, j + (offset * j)), Quaternion.identity, parent));
            }
        }
        float move = -0.5f * (gridSize - 1);
        parent.transform.position = new Vector3(move, 0f, move);
        
        ResetColor();
    }

    void ResetColor()
    {
        for (int i = 0; i < gos.Count; i++)
        {
            gos[i].GetComponent<MeshRenderer>().material = mats[0];
        }
    }
    
    
}


