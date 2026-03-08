using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for dash
/// </summary>
public class PlayerProjectileBody : MonoBehaviour
{
    HashSet<Collider> _hits = new HashSet<Collider>();
    [SerializeField] Transform myTransform;
    [SerializeField] Collider myCollider;
    
    public bool IsActive
    {
        set
        {
            _hits.Clear();
            myCollider.enabled = value;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (_hits.Contains(other)) return;
        _hits.Add(other);
        if (other.TryGetComponent(out E_Loco en))
        {
            en.PushMe(myTransform.position - myTransform.forward);
        }
    }
}
