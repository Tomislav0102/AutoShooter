using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpellGroup : MonoBehaviour
{
    public Transform myTransform;
    protected Brain owner;
    protected MyDuo<SpellMain, PassDataContainer> prefabsAndData;

    
    public virtual void InitializeMe(Brain ownersBrain, MyDuo<SpellMain, PassDataContainer> duo)
    {
        owner = ownersBrain;
        prefabsAndData = duo;
    }
}
