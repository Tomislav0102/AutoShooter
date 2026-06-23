using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Test : SerializedMonoBehaviour
{
    bool _started = false;
    float _timer;
    [Button]
    void Generate()
    {
        _started = true;
        _timer = 0;
    }

    void Update()
    {
        if (!_started) return;
        _timer += Time.deltaTime;
        transform.position += _timer * Vector3.forward;
        if (_timer > 1f)
        {
            _started = false;
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


