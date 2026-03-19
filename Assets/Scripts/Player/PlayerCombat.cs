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
        }
    }
    [SerializeField] float specialCooldownTime;
    protected int counterHit;
    
    void OnEnable()
    {
        EventBus.OnSpecialActivated += CallEv_OnSpecialActivated;
    }
    void OnDisable()
    {
        EventBus.OnSpecialActivated -= CallEv_OnSpecialActivated;
    }
    protected virtual void CallEv_OnSpecialActivated()
    {
        
    }

    public virtual void HealthHitCallback(DamageData damageData)
    {
        counterHit++;
    }


}
