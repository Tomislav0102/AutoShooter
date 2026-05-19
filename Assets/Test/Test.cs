using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class Test : MonoBehaviour
{
    public Transform target;
    public float speed;
    public ParticleSystem ps, psDark;
    
    [Button]
    void TestMethod()
    {
        ps.Stop();
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.y = speed;
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeDark = psDark.velocityOverLifetime;
        velocityOverLifetimeDark.y = speed;
        target.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        target.position = Vector3.zero;
        target.GetComponent<Rigidbody>().linearVelocity = speed * Vector3.forward;
        ps.Play();
    }

}


