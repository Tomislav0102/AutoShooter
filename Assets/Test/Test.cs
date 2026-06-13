using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Serialization;

public class Test : SerializedMonoBehaviour
{
    public GameObject[] children;
    public bool fullCircle = true;
    [Range(0, 359), HideIf(nameof(fullCircle))] public int arc = 359;
    [Range(1, 25)] public int numVisible = 1;
    
    
    [Button]
    void Generate()
    {
        // for (int i = 0; i < children.Length; i++)
        // {
        //     children[i].SetActive(false);
        // }
        // Transform[] transforms = new Transform[numVisible];
        // for (int i = 0; i < transforms.Length; i++)
        // {
        //     children[i].SetActive(true);
        //     transforms[i] = children[i].GetComponent<Transform>();
        // }
        // RadialSpread(transforms, fullCircle, arc);
    }

    void HalfCircle()
    {
        float angle = arc / (float)(numVisible + 1);
        for (int i = 0; i < children.Length; i++)
        {
            children[i].transform.rotation = Quaternion.identity;
            if (i < numVisible)
            {
                children[i].SetActive(true);
                children[i].transform.rotation = Quaternion.Euler(0f, angle * (i + 1) - arc * 0.5f, 0f);
            }
            else children[i].SetActive(false);
        }
    }

    void FullCircle()
    {
        float angle = 360 / (float)(numVisible);
        for (int i = 0; i < children.Length; i++)
        {
            children[i].transform.rotation = Quaternion.identity;
            if (i < numVisible)
            {
                children[i].SetActive(true);
                children[i].transform.rotation = Quaternion.Euler(0f, angle * (i + 1), 0f);
            }
            else children[i].SetActive(false);
        }
    }

}


