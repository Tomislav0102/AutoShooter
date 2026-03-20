using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    public Transform startTr, endTr;

    void FixedUpdate()
    {
        SphereCast();
    }

    [Button]
    void TestMethod()
    {
        
    }

    void SphereCast()
    {
        Vector3 direction = endTr.position - startTr.position;
        direction.y = 0f;
        if (Physics.SphereCast(startTr.position, startTr.localScale.x * 0.5f, direction.normalized, out RaycastHit hit, direction.magnitude))
        {
            print(hit.collider.name);
        }
    }
}


// Spell sp = Instantiate(Ga.me.spells.damageZone).GetComponent<Spell>();
// sp.InitializeMe(transform, Faction.Ally);
// sp.myTransform.position = transform.position;



// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
