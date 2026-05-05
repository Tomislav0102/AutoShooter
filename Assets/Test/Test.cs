using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using TMPro;

public class Test : MonoBehaviour
{
    public string layName;
    public int endLayerInteger;
    public Collider[] colliders;
    [Button]
    void TestMethod()
    {
        colliders = Physics.OverlapSphere(transform.position, 1, FinalInteger(layName));
    }

    int FinalInteger(string layerName)
    {
        int lay = LayerMask.NameToLayer(layerName);
        return  (1 << lay);
    }

}



// layerMask = (1 << layer); //set layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
