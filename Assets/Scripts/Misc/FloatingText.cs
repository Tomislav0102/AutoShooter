using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI myText;
    Color _startColor = Color.white;
    Color _endColor = Color.clear;
    [SerializeField] RectTransform myTransform;
    bool _started;
    float _lifeTime = 3f;
    float _timer;
    Vector3 _startPosition;
    float _startingOffsetY;
    [SerializeField] Image myIcon;

    
    
    public void SpawnMe(DamageData dam, float offsetY = 2f)
    {
        if (dam.damage >= 0) 
        {
            myIcon.sprite = Ga.me.gameData.GetElement(dam.element).sprite;
            _startColor = Ga.me.gameData.GetElement(dam.element).col;
            myText.text = dam.damage.ToString();
        }
        else // healing
        {
            myIcon.enabled = false;
            _startColor = Ga.me.gameData.colHeal;
            myText.text = (-dam.damage).ToString();
        }
        myText.color = _startColor;
        myIcon.color = _startColor;
        myIcon.enabled = dam.element != Element.Physical;
        _startPosition = dam.attacker.position;
        _startingOffsetY = offsetY;
        float xOffset = (myText.rectTransform.sizeDelta.x + myIcon.rectTransform.sizeDelta.x) * 0.5f ;
        Vector2 newPos = new Vector2(myText.rectTransform.anchoredPosition.x - xOffset, myText.rectTransform.anchoredPosition.y);
        myIcon.rectTransform.anchoredPosition = newPos;

        _started = true;
    }
    
    void LateUpdate()
    {
        if (!_started) return;
        
        _timer += Time.deltaTime;
        float moveY = _startingOffsetY + _timer * 5f;
        Vector3 targetPos = new Vector3(_startPosition.x, moveY, _startPosition.z);
        myTransform.position = Ga.me.cam.WorldToScreenPoint(targetPos);
        myText.color = myIcon.color = Color.Lerp(_startColor, _endColor, _timer / _lifeTime);
        if (_timer >= _lifeTime) EndMe();
    }

    void EndMe()
    {
        _started = false;
        
        Destroy(gameObject);
    }
}
