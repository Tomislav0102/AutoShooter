using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using TMPro;

public class Test : MonoBehaviour
{
    public List<int> startList;
    public Animator anim;
    [Button]
    void TestMethod()
    {
        startList = Utils.RandomListByType(startList);
    }

}



// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
