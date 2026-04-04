using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    public float f1;
    public int seconds;
    public float result;
    
    
    [Button]
    void TestMethod()
    {
        result = f1 % seconds;
    }


}




// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
