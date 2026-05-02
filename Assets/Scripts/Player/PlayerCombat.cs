using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerCombat : Combat
{
    [ShowInInspector, ReadOnly] CompSpellContainer _rotatingSwords;
    [SerializeField] int engageRange = 10;
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            Ga.me.specialUi.InitializeMe(specialCooldownTime);
            value.loco.lookAtTarget = true;
            _pLoco = Br.loco as P_Loco;
            
        }
    }

    public override Transform MyTarget
    {
        get => base.MyTarget;
        set
        {
            base.MyTarget = value;
            if (value == null)
            {
                _pLoco.Disp = Disposition.Relaxed;
                return;
            }

            _pLoco.Disp = distanceToTarget > engageRange ? Disposition.Wary : Disposition.Fighting;
        }
    }
    [SerializeField] float specialCooldownTime;
    P_Loco _pLoco;
    
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
        counterHitReceived++;
    }


}
