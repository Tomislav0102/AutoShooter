using UnityEngine;
using UnityEngine.EventSystems;

public class CameraButtons : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] GenSide side;
    
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Ga.me.camRig.rotatingTo = side;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Ga.me.camRig.rotatingTo = GenSide.Center;
    }
}
