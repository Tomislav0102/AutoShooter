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
    void LateUpdate()
    {
        playerFocus.localPosition = focusOffset * Vector3.up;
        Utils.CameraFollowAsymptotic(Ga.me.team.playerTransform.position, myTransform);
    }
    
    
    public void BtnCameraYaw(bool isLeftDirection)// can't use buttons, will need IPointerEnter interface with a new Monobehaviour for every UI element
    {
        int dir = isLeftDirection ? -1 : 1;
        myTransform.RotateAround(playerFocus.position, Vector3.up, dir * yawSpeed * Time.deltaTime);
    }

}
