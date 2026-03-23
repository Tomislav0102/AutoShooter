using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;
using DG.Tweening;

public class P_Loco : Loco
{
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            Ga.me.playerTransform = Br.myTransform;
            IsReady = true;
        }
    }

    public Disposition Disp
    {
        get => _disp;
        set
        {
            if (_disp != value)
            {
                _disp = value;
                
                anim.SetLayerWeight(1, 1);
                AttInputEnemy(false); 
                rotationConstraint.weight = 0;
                switch (value)
                {
                    case Disposition.Relaxed:
                        anim.SetLayerWeight(1, 0);
                        break;
                    case Disposition.Wary:
                        break;
                    case Disposition.Fighting:
                        AttInputEnemy(true);
                        rotationConstraint.weight = 1;
                        break;
                }
            }
        }
    }
    Disposition _disp;

    public override void CastSpell(bool isCasting = true)
    {
        anim.SetTrigger("cast");
    }

    void FixedUpdate()
    {
        Utils.CameraFollowAsymptotic(Br.myTransform.position, Ga.me.cameraRigTransform);
        float camAngle = Ga.me.cameraRigTransform.eulerAngles.y;
        Vector2 val = Quaternion.Euler(0, 0, -camAngle) * Ga.me.joystick.value;
        float dotVer = Vector3.Dot(Utils.MakeV3(val), Br.myTransform.forward);
        float dotHor = Vector3.Dot(Utils.MakeV3(val), Br.myTransform.right);
        Direction_Move(dotHor, dotVer);

        Br.myRigid.AddForce(1000 * Utils.MakeV3(moveSpeed * val));
        LookAtMethod(Utils.MakeV3(val));

    }

}
