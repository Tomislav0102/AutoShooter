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
    public Brain brain;
    public BuffEffects[] buffEffects;
    [Button]
    void AddS()
    {
        for (int i = 0; i < buffEffects.Length; i++)
        {
            brain.status.StatusInjectData(GenChange.Add, buffEffects[i]);

        }
    }
    [Button]
    void Remove()
    {
        for (int i = 0; i < buffEffects.Length; i++)
        {
            brain.status.StatusInjectData(GenChange.Remove, buffEffects[i]);

        }
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


