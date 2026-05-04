using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using TMPro;

public class Test : MonoBehaviour
{
    public string someLayerName;
    [Button]
    void TestMethod()
    {
        print(LayerMask.GetMask(new string[]{"Default", "Ground"}));
    }

}



// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
