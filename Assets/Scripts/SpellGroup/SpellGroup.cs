using UnityEngine;

public class SpellGroup : MonoBehaviour
{
    protected SpellControl[] mySpells;
    protected Transform myTransform;
    protected Brain brain;

    public virtual void InitializeMe(Brain ownersBrain)
    {
        myTransform = transform;
        brain = ownersBrain;
        mySpells = Utils.AllChildren<SpellControl>(myTransform);
    }

}
