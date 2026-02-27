using UnityEngine;

public class PlProfession : EventBus
{
    protected ICharacter parentCharacter;
    public bool isAttacking;
    [SerializeField] protected float damage;

    public void InitializeMe(ICharacter character)
    {
        parentCharacter = character;
    }

    public virtual void AttackAnimEvent()
    {
    }
}
