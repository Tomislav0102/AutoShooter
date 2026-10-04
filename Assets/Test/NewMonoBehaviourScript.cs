using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public ParticleSystem[] ps;
    public bool[] played;


    void Update()
    {
        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i].isPlaying) played[i] = true;
        }
    }
}
