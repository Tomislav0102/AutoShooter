using System;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public Camera cam;
    public Transform camTransform;
    public Transform myTransform;
    [SerializeField] Transform playerFocus;
    [SerializeField] int focusOffset = 10;
    [SerializeField] int yawSpeed = 100;
    public GenSide rotatingTo = GenSide.Center;

    void Start()
    {
        camTransform.position += focusOffset * Vector3.forward;
    }

    void LateUpdate()
    {
        if (Ga.me.team.playersBrain == null) return;
        Utils.CameraFollowAsymptotic(Ga.me.team.playersBrain.myTransform.position, myTransform);
        int dir = (int)rotatingTo - 1;
        myTransform.Rotate(dir * yawSpeed * Time.deltaTime * Vector3.up);
    }

}
