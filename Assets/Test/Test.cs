using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    public Transform tr;
    public float dot;
    public Vector2 dir;
    
    
    [Button]
    void TestEvent()
    {
       
    }
    [Button]
    void ClearEvent()
    {
      
    }

    void Update()
    {
      //  dot = Vector2.Dot(Utils.MakeV2(tr.forward), Utils.MakeV2(Vector3.forward));
      //dir = (Utils.MakeV2(transform.position) - Utils.MakeV2(tr.position)).normalized;
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


