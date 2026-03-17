using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class P_Loco : Loco
{
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            Ga.me.playerTransform = myTransform;
            IsReady = true;
        }
    }


    void FixedUpdate()
    {
        Utils.CameraFollowAsymptotic(myTransform.position, Ga.me.cameraRigTransform);
        float camAngle = Ga.me.cameraRigTransform.eulerAngles.y;
        Vector2 val = Quaternion.Euler(0, 0, -camAngle) * Ga.me.joystick.value;
        float dotVer = Vector3.Dot(Utils.MakeV3(val), myTransform.forward);
        float dotHor = Vector3.Dot(Utils.MakeV3(val), myTransform.right);
        Direction_Move(dotHor, dotVer);

        Vector3 finalVelocity;
        if (isDashing)
        {
            finalVelocity = velocityModifier * CONST_DASH_VELOCITY_MAX * myTransform.forward;
        }
        else
        {
            finalVelocity = velocityModifier * Utils.MakeV3(moveSpeed * val);
            LookAtMethod();
        }
        Br.myRigid.linearVelocity = finalVelocity;

    }

}
