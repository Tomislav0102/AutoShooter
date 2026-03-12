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
            IsReady = true;

        }
    }
    const float CONST_CamEdgeBottom = 7f;

    void FixedUpdate()
    {
        Vector3 targetPos = Vector3.Lerp(gm.cameraRigTransform.position, myTransform.position, 0.2f); 
        //targetPos.z = Mathf.Max(targetPos.z, CONST_CamEdgeBottom);
        gm.cameraRigTransform.position = targetPos;

        float camAngle = gm.cameraRigTransform.eulerAngles.y;
        Vector2 val = Quaternion.Euler(0, 0, -camAngle) * gm.joystick.value;
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
