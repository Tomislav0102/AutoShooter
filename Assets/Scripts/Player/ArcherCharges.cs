using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class ArcherCharges : MonoBehaviour
{
    [SerializeField] ParticleSystem[] ps;


    public void InitializeMe(HashSet<Archer.Charge> charges)
    {
        foreach (Archer.Charge item in charges)
        {
            ps[(int)item].Play();
        }
    }
}
