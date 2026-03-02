using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    public GameManager gm;

    [Button]
    void TestMethod()
    {
       
    }


    void OnTriggerEnter(Collider other)
    {
        print(other.name);
    }
}

