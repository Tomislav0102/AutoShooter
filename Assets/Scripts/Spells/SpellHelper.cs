using UnityEngine;
using Sirenix.OdinInspector;

public class SpellHelper : MonoBehaviour
{
    protected Transform myTransform;
    protected Brain brain;
    protected Spell spellPrefabToInstantiate;
    protected float damage;
    [ReadOnly] public bool isActive;


    public virtual void InitializeMe(Brain br, Spell spellPrefab, float dam)
    {
        myTransform = transform;
        brain = br;
        spellPrefabToInstantiate = spellPrefab;
        damage = dam;
    }
}