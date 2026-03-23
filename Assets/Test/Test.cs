using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    public Rigidbody obj;
    public float force;
    [Button]
    void TestMethod()
    {
        transform.rotation *=Quaternion.Euler(45, 45, 0);
    }

}



        // Vector3 throwDirection = new Vector3(-45f, Random.Range(0f, 360f), 0f);
        // Rigidbody rb = Instantiate(obj, transform.position, Quaternion.identity);
        // transform.eulerAngles = throwDirection;
        // rb.AddForce(force * transform.forward, ForceMode.Impulse);

// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
