using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    public static System.Action someAction;


    void Start()
    {
        someAction += () =>
        {
            print("someAction");
        };
    }

    [Button]
    void TestMethod()
    {
        someAction?.Invoke();
    }
    


}



// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
