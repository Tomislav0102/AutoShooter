using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for dash
/// </summary>
public class BodyBullet : MonoBehaviour
{
    [SerializeField] Transform myTransform;
    [SerializeField] Collider myCollider;
    [Range(0, 20)] [SerializeField] int knockBack = 1;
    public bool IsActive
    {
        set
        {
            myCollider.enabled = value;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Brain brain))
        {
           if (brain.loco != null) brain.loco.KnockBack(Utils.Direction(myTransform.position, other.transform
               .position), knockBack);
        }
    }
}
