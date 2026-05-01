using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using TMPro;

public class Test : MonoBehaviour
{
    public GameObject[] pooledCubes;
    public GameObject myCube;
    
    [Button]
    void TestMethod()
    {
        for (int i = 0; i < pooledCubes.Length; i++)
        {
            print($"{i} {Equals(pooledCubes[i],  myCube)}");
        }
    }

}



// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
