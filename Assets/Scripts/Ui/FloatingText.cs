using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    Color _endColor = Color.clear;
    
    [SerializeField] RectTransform myTransform;
    [SerializeField] TextMeshProUGUI[] myTexts;
    Color[] _startColors;
    bool _initialized;
    float _lifeTime = 3f;
    float _timer;
    Vector3 _startPosition;
    float _startingOffsetY;
    int _widthSingle = 100;
    
    public void SpawnMe(MyDuo<Element, float> damage, float offsetY = 2f)
    {
        _startColors = new Color[myTexts.Length];
        MyDuo<Element, float> effDamage = new MyDuo<Element, float>();
        for (int i = 0; i < damage.Length(); i++)
        {
            if (damage.GetValue(i) != 0f) effDamage.Add(damage.GetKey(i), damage.GetValue(i));
        }
        for (int i = 0; i < effDamage.Length(); i++)
        {
            Element k = effDamage.GetKey(i);
            float v = effDamage.GetValue(i);
            int index = (int)k;
            myTexts[index].enabled = true;
            if (v > 0)
            {
                myTexts[index].text = $" <sprite index={index}>{v} ";
                _startColors[index] = Ga.me.gameData.GetElement((Element)index).col;
            }
            else if (v < 0) // healing
            {
                myTexts[index].text =(-v).ToString("0");
                _startColors[index] = Ga.me.gameData.colHeal;
            }

        }
        
        GetComponent<RectTransform>().sizeDelta = new Vector2(_widthSingle * effDamage.Length(), 0);
        GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
        _startPosition = myTransform.position; 
        _startingOffsetY = offsetY;
        
        _initialized = true;
    }    
    public void SpawnMe(string st, Color col, float offsetY = 2f)
    {
        _startColors = new Color[myTexts.Length];
        myTexts[0].enabled = true;
        myTexts[0].text = st;
        _startColors[0] = col;
        GetComponent<RectTransform>().sizeDelta = new Vector2(_widthSingle, 0);
        GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
        _startPosition = myTransform.position; 
        _startingOffsetY = offsetY;
        
        _initialized = true;

    }
    
    void LateUpdate()
    {
        if (!_initialized) return;
        
        _timer += Time.deltaTime;
        float moveY = _startingOffsetY + _timer * 5f;
        Vector3 targetPos = new Vector3(_startPosition.x, moveY, _startPosition.z);
        myTransform.position = Ga.me.camRig.cam.WorldToScreenPoint(targetPos);
        for (int i = 0; i < myTexts.Length; i++)
        {
            myTexts[i].color = Color.Lerp(_startColors[i], _endColor, _timer / _lifeTime);
        }
        if (_timer >= _lifeTime) EndMe();
    }

    void EndMe()
    {
        _initialized = false;
        
        Destroy(gameObject);
    }
}
