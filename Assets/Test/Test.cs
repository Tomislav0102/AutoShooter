using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class Test : SerializedMonoBehaviour
{
    public RectTransform rt;
    [Button]
    void Metoda()
    {
        print(Screen.width);
    }
}


