using UnityEngine;

public class PlayerCombat : Combat
{
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            Ga.me.specialUi.InitializeMe(specialCooldownTime);
            value.loco.lookAtTarget = true;
        }
    }
    [SerializeField] float specialCooldownTime;
    protected int counterHit;
    
    
    void OnEnable()
    {
        EventBus.OnUltimateActivated += CallEv_OnUltimateActivated;
    }
    void OnDisable()
    {
        EventBus.OnUltimateActivated -= CallEv_OnUltimateActivated;
    }
    protected virtual void CallEv_OnUltimateActivated()
    {
        
    }

    public virtual void HealthHitCallback(InjectHealth injectHealth)
    {
        counterHit++;
    }


}
