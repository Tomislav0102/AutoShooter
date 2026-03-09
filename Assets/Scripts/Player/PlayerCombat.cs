using UnityEngine;

public class PlayerCombat : Combat
{
    [SerializeField] float specialCooldownTime;
    
    public override void Initialize(Brain brain)
    {
        base.Initialize(brain);
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


}
