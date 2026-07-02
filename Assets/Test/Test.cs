using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Test : SerializedMonoBehaviour
{
    public Animator anim;
    public float velMag;
    [Button]
    void Generate()
    {
        anim.applyRootMotion = true;
        anim.SetTrigger("roll");
    }
    [Button]
    void ResetPosition()
    {
        anim.transform.localPosition = Vector3.zero;
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


