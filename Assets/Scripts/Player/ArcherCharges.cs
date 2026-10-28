using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ArcherCharges : MonoBehaviour
{
    [SerializeField] GameObject[] chargeGos;


    public void InitializeMe(HashSet<Archer.Charge> charges)
    {
        Utils.ActivateOneArrayElement(chargeGos);
        List<Transform> chargesToDisplay = new List<Transform>();
        foreach (Archer.Charge item in charges)
        {
            chargeGos[(int)item].SetActive(true);
            chargesToDisplay.Add(chargeGos[(int)item].transform);
        }

        Vector2 vectorRange = new Vector2(-0.2f, .3f);
        float range = Mathf.Abs( vectorRange.x - vectorRange.y);
        for (int i = 0; i < chargesToDisplay.Count; i++)
        {
            float z = range * (float)(i + 1) / (chargesToDisplay.Count + 1) + vectorRange.x;
            chargesToDisplay[i].localPosition = z * Vector3.forward;
        }
    }
}
