using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{

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
