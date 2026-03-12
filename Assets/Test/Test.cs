using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    public GameManager gm;
    public int layer;
    public LayerMask layerMask;

    [Button]
    void TestMethod()
    {
        print(layerMask.value);
    }

}






// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
