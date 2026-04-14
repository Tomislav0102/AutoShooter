using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// This character is on hold because I can't find good attack animation.
/// Current one can't be used with animation events
/// </summary>
public class P_Barb : PlayerCombat
{
    [SerializeField] Transform weaponTr, weaponLookAtTarget;
    Disposition _disposition;
    [ShowInInspector, ReadOnly] float _weaponAngle;


    public void DispositionFromLoco(Disposition disp)
    {
     _disposition = disp;   
    }

    protected override void Update()
    {
        base.Update();
        switch (_disposition)
        {
            case Disposition.Fighting:
                break;
        }

        _weaponAngle = Vector2.Angle(weaponTr.forward, Br.myTransform.forward);
    }

    void LateUpdate()
    {
       // weaponTr.LookAt(weaponLookAtTarget);
        
    }
}
