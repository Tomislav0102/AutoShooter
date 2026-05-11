using UnityEngine;
using Sirenix.OdinInspector;

public class SpellGroup : MonoBehaviour
{
    protected SpellControl[] mySpells;
    public Transform myTransform;
    protected Brain brain;

    public virtual void InitializeMe(Brain ownersBrain)
    {
        brain = ownersBrain;
        mySpells = Utils.AllChildren<SpellControl>(myTransform);
    }

}
