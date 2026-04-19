using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using TMPro;

public class Test : MonoBehaviour
{
    public TextMeshProUGUI someText;
    public int index;

    void Start()
    {
    }

    [Button]
    void TestMethod()
    {
        someText.text = $"<sprite index=0>555 <sprite index=1>124 <sprite index=2>444 <sprite index=3>124\n <sprite index=4>124 <sprite index=5>124 <sprite index=6>124";
    }
    


}



// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
