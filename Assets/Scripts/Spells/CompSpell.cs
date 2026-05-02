using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CompSpell : MonoBehaviour
{
    public Transform myTransform;
    public Rigidbody myRigid;
    public SphereCollider mySphereCollider;
    public CapsuleCollider myCapsuleCollider;
    public SphereCollider mySolidSphereCollider;
    public Transform myMesh;
    public SpriteRenderer warningRend;
    public Transform visualization;
    public System.Action onTrigEnter;
    public System.Action onTrigExit;

    void OnTriggerEnter(Collider other)
    {
        onTrigEnter?.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        onTrigExit?.Invoke();
    }
}
