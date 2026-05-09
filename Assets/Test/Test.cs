using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using TMPro;

public class Test : MonoBehaviour
{
    public float speed;
    public Rigidbody rbPrefab;
    Rigidbody _rb;
    
    [Button]
    void TestMethod()
    {
        if (_rb != null) Destroy(_rb.gameObject);
        transform.rotation = Quaternion.Euler(-45f, Random.Range(0f, 360f), 0);
        _rb = Instantiate(rbPrefab, transform.position, transform.rotation);
        _rb.AddForce(speed * transform.forward, ForceMode.VelocityChange);

    }

}


