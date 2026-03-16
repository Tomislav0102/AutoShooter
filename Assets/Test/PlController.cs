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
            myRigid.linearVelocity = speed * Utils.MakeV3(GameManager.Instance.joystick.value);
        }
    }
}

