using UnityEngine;
using Sirenix.OdinInspector;

public class SpellHelper : MonoBehaviour
{
    protected Brain brain;
    protected Spell spellPrefabToInstantiate;
    [ReadOnly] public bool isActive;


    public virtual void InitializeMe(Brain br, Spell spellPrefab)
    {
        brain = br;
        this.spellPrefabToInstantiate = spellPrefab;
    }
}