using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCombat : Combat
{
    [SerializeField] int engageRange = 10;
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            Ga.me.ultimateUi.InitializeMe(ultimateCooldownTime);
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
    [SerializeField] float ultimateCooldownTime;
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



}
