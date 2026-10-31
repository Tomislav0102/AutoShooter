using System;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public Camera cam;
    public Transform camTransform;
    public Transform myTransform;
    [SerializeField] int focusOffset = -4;
    [SerializeField] int yawSpeed = 100;
    public GenSide rotatingTo = GenSide.Center;

    void LateUpdate()
    {
        if (Ga.me.team.playersBrain == null) return;
        Vector3 followPos = focusOffset * myTransform.forward + Ga.me.team.playersBrain.myTransform.position;
        Utils.CameraFollowAsymptotic(followPos, myTransform);
        int dir = (int)rotatingTo - 1;
        myTransform.Rotate(dir * yawSpeed * Time.deltaTime * Vector3.up);
    }

}
