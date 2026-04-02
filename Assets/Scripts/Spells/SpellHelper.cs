using UnityEngine;
using Sirenix.OdinInspector;

public class SpellHelper : MonoBehaviour
{
    protected Transform myTransform;
    protected Brain brain;
    protected Spell spellPrefabToInstantiate;
    [ReadOnly] public bool isActive;


    public virtual void InitializeMe(Brain br, Spell spellPrefab)
    {
        myTransform = transform;
        brain = br;
        this.spellPrefabToInstantiate = spellPrefab;
    }
}