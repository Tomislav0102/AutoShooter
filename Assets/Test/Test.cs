using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Test : SerializedMonoBehaviour
{
    public float rotSpeed;
    bool _isRotating;
    [Button]
    void Generate()
    {
        ResetThing();
        _isRotating = !_isRotating;
    }
    [Button]
    void ResetThing()
    {
        _isRotating = false;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if(!_isRotating) return;
        transform.Rotate(rotSpeed * Time.deltaTime * Vector3.up);
    }

    void OnAnimatorMove()
    {
        
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


