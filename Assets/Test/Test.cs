using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class Test : SerializedMonoBehaviour
{
    public Canvas canvas;
    public RectTransform rt1, rt2;

    void Start()
    {
        rt1.anchoredPosition = new Vector2(Screen.width, Screen.height);
    }

    [Button]
    void Metoda()
    {
      //  print(canvas.pixelRect);
        //print($"{Screen.width}x{Screen.height}");
        rt1.anchoredPosition = new Vector2(Screen.width, Screen.height);
        //right edge
        // rt1.anchorMin = rt1.anchorMax = new Vector2(1, 0.5f);
        // rt1.pivot = new Vector2(1f, 0.5f);
        // rt1.anchoredPosition = new Vector2(0, rt1.anchoredPosition.y);
    }
}


