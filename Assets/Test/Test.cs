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
    public Character playerCharacter;
    public BuffStats buffStats;
    
    [Button]
    void AddS()
    {
        StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 3)
                {
                    yield return new WaitForSeconds(2);
                }
                print(i);
                yield return null;
            }
        }
    }
    [Button]
    void Remove()
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


