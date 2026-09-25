using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCombat : MonoBehaviour, IIniBrain, ITargetTracker
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
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
    public int engageRange = 10;
    [SerializeField] PlayerLoco playerLoco;


    void OnEnable()
    {
        Ga.OnUltimateActivated += CallEv_OnUltimateActivated;
    }
    void OnDisable()
    {
        Ga.OnUltimateActivated -= CallEv_OnUltimateActivated;
    }
    void CallEv_OnUltimateActivated()
    {
        Br.loco.AttackAnimation(AnimAttackType.Ultimate);
    }
}

