using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.Animations.Rigging;


public class PlayerLoco : MonoBehaviour, IIniBrain
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            Ga.me.team.playersBrain = value;
            value.agent.updateRotation = false;
        }
    }
    Brain _br;
    
    [SerializeField] MultiRotationConstraint rotationConstraint;
    [SerializeField] PlayerCombat playerCombat;
    
    [SerializeField] ParticleSystem weaponTrail;
    public Alertness Alert
    {
        set
        {
            if (value == _alertness) return;
            _alertness = value;
            Br.loco.anim.SetLayerWeight(1, 1);
            rotationConstraint.weight = 0;
            //  if (weaponTrail != null) weaponTrail.Stop();
            Br.loco.OvrOrientation = true;
            switch (_alertness)
            {
                case Alertness.Relaxed:
                    Br.loco.AttackAnimation(null);
                    Br.loco.OvrOrientation = false;
                    Br.loco.anim.SetLayerWeight(1, 0);
                    break;
                case Alertness.Alarmed:
                    Br.loco.AttackAnimation(null);
                    break;
                case Alertness.Fighting:
                    //  if (weaponTrail != null) weaponTrail.Play();
                    Br.loco.AttackAnimation(playerCombat.animAttackType);
                    rotationConstraint.weight = 1;
                    break;
            }

        }
    }
    [ShowInInspector, ReadOnly] Alertness _alertness;
    [HideInInspector] public Vector2 effJoystickValue;

    void Update()
    {
        float camAngle = Ga.me.camRig.myTransform.eulerAngles.y;
        effJoystickValue = Quaternion.Euler(0, 0, -camAngle) * Ga.me.uiManager.joystick.value;
        Vector3 myForward;
        if (Br.loco.OvrOrientation && Br.combat.MyTarget != null) myForward = Utils.Direction(Br.myTransform.position, Br.combat.MyTarget.myTransform.position);
        else myForward = Utils.MakeV3(effJoystickValue);
        Br.loco.Orientation(myForward);
        if (Br.loco.OvrMotion) return;
        
        float dotVer = Vector3.Dot(Utils.MakeV3(effJoystickValue), Br.myTransform.forward);
        float dotHor = Vector3.Dot(Utils.MakeV3(effJoystickValue), Br.myTransform.right);
        Br.loco.Direction_Move(dotHor, dotVer);
        Br.agent.velocity = Br.loco.moveSpeed * Utils.MakeV3(effJoystickValue);
    }
}
