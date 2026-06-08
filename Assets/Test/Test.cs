using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class Test : SerializedMonoBehaviour
{
    public Faction myFaction, targetFaction;
    public FactionToTarget factionToTarget;
    [Button]
    void Metoda()
    {
       print(Utils.CanTargetFaction(myFaction, targetFaction, factionToTarget));
    }
}


