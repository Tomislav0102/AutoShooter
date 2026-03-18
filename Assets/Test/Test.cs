using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    public Rigidbody rigid;
    public Vector3 dir;
    public float force;
    public InputActionReference jump;

    void OnEnable()
    {
        jump.action.Enable();
    }

    void Update()
    {
        if (jump.action.IsPressed())
        {
            rigid.AddForce(dir * force);
        }
    }

    [Button]
    void TestMethod()
    {
        
    }
}


// Spell sp = Instantiate(Ga.me.spells.damageZone).GetComponent<Spell>();
// sp.InitializeMe(transform, Faction.Ally);
// sp.myTransform.position = transform.position;



// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
