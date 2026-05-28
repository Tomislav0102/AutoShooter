using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class Test : SerializedMonoBehaviour
{
    public Transform[] allTargets;
    public Dictionary<Transform, float> dic = new Dictionary<Transform, float>();
    public List<Transform> results = new List<Transform>();

    [Button]
    void Reset()
    {
        dic = new Dictionary<Transform, float>();
        foreach (Transform t in allTargets)
        {
            dic.Add(t, Vector3.Distance(t.position, transform.position));
        }
        
    }
    [Button]
    void Sort()
    {
        Reset();
        var sorted = dic.ToList();
        sorted.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
        results.Clear();
        foreach (KeyValuePair<Transform, float> item in sorted)
        {
            results.Add(item.Key);
        }
    }

}


