using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace TomoJoystick
{
    public class Joystick : MonoBehaviour
    {
        [SerializeField] InputActionReference inputClick;
        [SerializeField] RectTransform knob, rayTarget, pointerRect;
        GameObject _rayTargetGo;
        RectTransform _myRect;
        float _maxRadius;
        [HideInInspector] public Vector2 value;
        
        void Awake()
        {
            _myRect = GetComponent<RectTransform>();
            _maxRadius = rayTarget.rect.width * 0.5f;
            _rayTargetGo = rayTarget.gameObject;
        }

        void OnEnable()
        {
            inputClick.action.Enable();
        }

        void Update()
        {
            float x = (knob.anchoredPosition.x - rayTarget.anchoredPosition.x) / _maxRadius;
            float y = (knob.anchoredPosition.y - rayTarget.anchoredPosition.y) / _maxRadius;
            value = new Vector2(x, y);
            
            GameObject currentGo = EventSystem.current.currentSelectedGameObject;
            bool canUse = currentGo == null || currentGo == _rayTargetGo;
            if (canUse && inputClick.action.WasPressedThisFrame())
            {
                _myRect.position = PointerPos();
            }
            SetKnob(canUse && inputClick.action.IsPressed());
        }

        void SetKnob(bool pressed)
        {
            if (pressed)
            {
                pointerRect.position = PointerPos();
                float distance = Vector2.Distance(pointerRect.anchoredPosition, rayTarget.anchoredPosition);
                distance = Mathf.Clamp(distance, 0, _maxRadius);
                Vector2 dir = (pointerRect.position - rayTarget.position).normalized;
                knob.anchoredPosition = dir * distance + rayTarget.anchoredPosition;
                return;
            }
            knob.anchoredPosition = Vector2.zero;

        }

        static Vector2 PointerPos()
        {
#if UNITY_EDITOR
            return Mouse.current.position.ReadValue();
#else
          if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.IsPressed())
          {
            return Touchscreen.current.primaryTouch.position.ReadValue();
          }
#endif

            return Vector2.zero;
        }
    }
}
