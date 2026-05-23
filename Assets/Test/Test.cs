using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class Test : MonoBehaviour
{
    public int num1;
    public int modulo;
    public int result;
    
    [Button]
    void TestMethod()
    {
        
    }

    void Update()
    {
        result = num1 % modulo;
    }
}


