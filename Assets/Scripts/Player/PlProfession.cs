using UnityEngine;

public class PlProfession : EventBus
{
    protected PlControl control;
    protected ICharacter parentCharacter;
    public bool isAttacking;
    [SerializeField] protected float damage;
    [SerializeField] float specialCooldownTime;
    
    public void InitializeMe(PlControl plControl)
    {
        control = plControl;
        parentCharacter = control.GetComponent<ICharacter>();
        gm.specialUi.InitializeMe(specialCooldownTime);
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.OnSpecialActivated += CallEv_OnSpecialActivated;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.OnSpecialActivated -= CallEv_OnSpecialActivated;
    }
    protected virtual void CallEv_OnSpecialActivated()
    {
        
    }


    public virtual void AttackAnimEvent()
    {
    }
}
