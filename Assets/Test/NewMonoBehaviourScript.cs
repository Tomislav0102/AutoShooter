using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speed = 1f;

    void Start()
    {
        SetSpeed(Vector2.up);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Vector3 dir =  transform.position - other.transform.position;
        dir.Normalize();
        SetSpeed(dir);
    }

    void SetSpeed(Vector2 dir)
    {
        GetComponent<Rigidbody2D>().linearVelocity = speed * dir;
    }

}
