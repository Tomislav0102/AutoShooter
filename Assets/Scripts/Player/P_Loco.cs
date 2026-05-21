using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class P_Loco : Loco
{
    [SerializeField] ParticleSystem weaponTrail;
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            Ga.me.playerTransform = value.myTransform;
            IsInitialized = true;
        }
    }

    public Disposition Disp
    {
        set
        {
            if (value == _disp) return;
            _disp = value;
            anim.SetLayerWeight(1, 1);
            AttInputEnemy(false);
            rotationConstraint.weight = 0; 
           // if (weaponTrail != null) weaponTrail.Stop();
            switch (value)
            {
                case Disposition.Relaxed:
                    anim.SetLayerWeight(1, 0);
                    break;
                case Disposition.Wary:
                    break;
                case Disposition.Fighting:
                  //  if (weaponTrail != null) weaponTrail.Play();
                    AttInputEnemy(true);
                    rotationConstraint.weight = 1;
                    break;
            }

        }
    }
    [ShowInInspector, ReadOnly] Disposition _disp;
    
    int _posId = Shader.PropertyToID("_Position");
    int _sizeID = Shader.PropertyToID("_Size");
    Transform _camTransform;

    void Awake()
    {
        _camTransform = Ga.me.cam.transform;
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
        
        if (Physics.Linecast(_camTransform.position, Br.myTransform.position, Utils.MyLayer(Ga.me.gameData.layWallsSeeThrough)))
        {
            Ga.me.gameData.matSeeThroughWalls.SetFloat(_sizeID, 0.5f);
        }
        else Ga.me.gameData.matSeeThroughWalls.SetFloat(_sizeID, 0);
    }


    protected override void CallEv_OnLevelLoaded()
    {
        base.CallEv_OnLevelLoaded();
        Physics.IgnoreCollision(Ga.me.LevelMan.ground.GetComponent<Collider>(), Br.myCollider);
    }
}
