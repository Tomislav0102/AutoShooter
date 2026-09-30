using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    public Brain br;
    public BuffEffects buffEffects;
    public BuffEffects buffEffects1;
  //  public BuffStats buffStats;
  public MyDuo<string, int> myDuo = new MyDuo<string, int>();
    
    [Button]
    void AddS()
    {
       br.status.StatusInjectData(GenChange.Add, buffEffects);
    }
    [Button]
    void Remove()
    {
       br.status.StatusInjectData(GenChange.Add, buffEffects1);
    }

}


public class AngledShot
{
    public Transform tr;
    public float maxHeight;
    public float distance;
    public float flightTime;
    public float posY, posZ, timer;

    void UpdateLoop()
    {
        timer += Time.deltaTime / flightTime;
        if (timer >= 1)
        {
            timer = 0f;
        }
        else if (timer > 0.5f)
        {
            posY = Mathf.Sqrt(1 - timer) * maxHeight * 2;
        }
        else
        {
            posY = Mathf.Sqrt(timer) * maxHeight * 2;
        }
        posZ += Time.deltaTime * distance;
        tr.position = new Vector3(0f, posY, posZ);
    }
}


