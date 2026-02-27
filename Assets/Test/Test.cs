using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform target;
    public RectTransform rectFollow;
    public Camera cam;
    public Vector3 offset = new Vector3(0, 2, 0);

    void LateUpdate()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);
        
        //hide if target is behind camera
        rectFollow.gameObject.SetActive(screenPos.z > 0);
        
        rectFollow.position = screenPos;
    }
}

