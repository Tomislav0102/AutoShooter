using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCombat : MonoBehaviour, IInitialization, ITargetTracker
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            Ga.me.ultimateUi.InitializeMe(ultimateCooldownTime);
        }
    }
    Brain _br;
    public Transform MyTarget
    {
        get => _myTarget;
        set
        {
            _myTarget = value;
            if (value == null)
            {
                playerLoco.Alert = Alertness.Relaxed;
                return;
            }
            playerLoco.Alert = Br.combat.distanceToTarget > engageRange ? Alertness.Alarmed : Alertness.Fighting;
        }
    }
    Transform _myTarget;

    public AnimAttackType animAttackType;
    [SerializeField] int engageRange = 10;
    [SerializeField] float ultimateCooldownTime;
    [SerializeField] PlayerLoco playerLoco;
    

    void OnEnable()
    {
        EventBus.OnUltimateActivated += CallEv_OnUltimateActivated;
    }
    void OnDisable()
    {
        EventBus.OnUltimateActivated -= CallEv_OnUltimateActivated;
    }
    void CallEv_OnUltimateActivated()
    {
        Br.loco.AttackAnimation(AnimAttackType.Ultimate);
    }



}
