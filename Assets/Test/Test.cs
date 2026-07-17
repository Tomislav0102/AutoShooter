using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.AI;

public class Test : SerializedMonoBehaviour
{
    public Transform cube, sphere;
    public float speed = 1f;
    
    [Button]
    void GenerateEvent()
    {
    }
    [Button]
    void InvokeEvent()
    {
        
    }
    [Button]
    void ClearEvent()
    {
       
    }


    void Update()
    {
        cube.RotateAround(sphere.position, Vector3.one, speed * Time.deltaTime);
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


