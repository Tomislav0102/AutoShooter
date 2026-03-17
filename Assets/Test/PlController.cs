using System;
using UnityEngine;

namespace TestApp
{

    public class PlController : MonoBehaviour
    {
        public Rigidbody myRigid;
        public float speed = 10;
        void FixedUpdate()
        {
            float camAngle = GameManager.Instance.camRigTransform.eulerAngles.y;
            Vector2 valV2 = Quaternion.Euler(0, 0, -camAngle) * GameManager.Instance.joystick.value;

            Vector3 vel = speed * Utils.MakeV3(valV2);
            vel.y = Physics.gravity.y;
            myRigid.linearVelocity = vel;
            Utils.CameraFollowAsymptotic(transform.position, GameManager.Instance.camRigTransform);
        }
    }
}

