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
    public ParticleSystem[] ps;
    public Color col;
    [Button]
    void AddS()
    {
    }
    void SetBuff(GenChange genChange)
    {
        for (int i = 0; i < buffEffects.Length; i++)
        {
            brain.status.StatusInjectData(genChange, buffEffects[i]);
        }
    }
    [Button]
    void Remove()
    {
    }

    // void Start()
    // {
    //     for (int i = 0; i < 10; i++)
    //     {
    //         Vector2 v2 = new Vector2(0, 1);
    //         v2 = Utils.RotateV2(v2, Random.Range(0, 360f));
    //         print(v2.magnitude);
    //         
    //     }
    // }

    // void Update()
    // {
    //     for (int i = 0; i < ps.Length; i++)
    //     {
    //         ParticleSystem.MainModule main = ps[i].main;
    //         main.startColor = col;
    //         
    //     }
    // }
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


