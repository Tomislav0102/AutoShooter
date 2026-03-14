using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    Ga _ga;

    void Awake()
    {
        _ga = Ga.me;
        print(_ga.name);
    }

    [Button]
    void TestMethod()
    {
        
    }

}






// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
