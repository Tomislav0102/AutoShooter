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
            pLoco = Br.loco as P_Loco;
            
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
                pLoco.Disp = Disposition.Relaxed;
                return;
            }

            pLoco.Disp = distanceToTarget > engageRange ? Disposition.Wary : Disposition.Fighting;
        }
    }
    [SerializeField] float ultimateCooldownTime;
    protected P_Loco pLoco;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.OnUltimateActivated += CallEv_OnUltimateActivated;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.OnUltimateActivated -= CallEv_OnUltimateActivated;
    }
    void CallEv_OnUltimateActivated()
    {
        Br.loco.CastSpell();
    }



}
