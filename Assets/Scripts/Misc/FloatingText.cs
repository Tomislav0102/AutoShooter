using System;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI myText;
    Color _startColor = Color.white;
    Color _endColor = Color.clear;
    [SerializeField] RectTransform myTransform;
    bool _started;
    float _lifeTime = 3f;
    float _timer;
    Transform _target;
    float _startingOffsetY;

    public void SpawnMe(Transform target, float offsetY, string text, Color color)
    {
        myText.text = text;
        _startColor = color;
        _started = true;
        _target = target;
        _startingOffsetY = offsetY;
    }

    void LateUpdate()
    {
        if (!_started) return;
        if (_target == null)
        {
            EndMe();
            return;
        }
        
        _timer += Time.deltaTime;
        Vector3 moveY = new Vector3(0f, _startingOffsetY + _timer * 5f, 0f);
        Vector3 screenPos = Ga.me.cam.WorldToScreenPoint(_target.position + moveY);
        myTransform.position = screenPos;
        myText.color = Color.Lerp(_startColor, _endColor, _timer / _lifeTime);
        if (_timer >= _lifeTime) EndMe();
    }

    void EndMe()
    {
        _started = false;
        
        Destroy(gameObject);
    }
}
