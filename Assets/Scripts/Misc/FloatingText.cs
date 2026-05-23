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
    bool _initialized;
    float _lifeTime = 3f;
    float _timer;
    Vector3 _startPosition;
    float _startingOffsetY;
    int _widthSingle = 100;
    
    
    public void SpawnMe(InjectHealth dam, float offsetY = 2f)
    {
        foreach (KeyValuePair<Element, float> item in dam.damage)
        {
            int index = (int)item.Key;
            myTexts[index].enabled = true;
            if (item.Value >= 0)
            {
                myTexts[index].text = $" <sprite index={index}>{item.Value} ";
                myTexts[index].color = Ga.me.gameData.GetElement((Element)index).col;
            }
            else // healing
            {
                myTexts[index].text =(-item.Value).ToString("0");
                myTexts[index].color = Ga.me.gameData.colHeal;
            }
        }
        
        GetComponent<RectTransform>().sizeDelta = new Vector2(_widthSingle * dam.damage.Count, 0);
        GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
        _startPosition = transform.position; 
        _startingOffsetY = offsetY;
        
        _initialized = true;
    }    
    public void SpawnMe(string st, Color col, float offsetY = 2f)
    {
        myTexts[0].enabled = true;
        myTexts[0].text = st;
        myTexts[0].color = col;
        GetComponent<RectTransform>().sizeDelta = new Vector2(_widthSingle, 0);
        GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
        _startPosition = transform.position; 
        _startingOffsetY = offsetY;
        
        _initialized = true;

    }
    
    void LateUpdate()
    {
        if (!_initialized) return;
        
        _timer += Time.deltaTime;
        float moveY = _startingOffsetY + _timer * 5f;
        Vector3 targetPos = new Vector3(_startPosition.x, moveY, _startPosition.z);
        myTransform.position = Ga.me.cam.WorldToScreenPoint(targetPos);
        for (int i = 0; i < myTexts.Length; i++)
        {
            myTexts[i].color = Color.Lerp(Ga.me.gameData.GetElement((Element)i).col, _endColor, _timer / _lifeTime);
        }
        if (_timer >= _lifeTime) EndMe();
    }

    void EndMe()
    {
        _initialized = false;
        
        Destroy(gameObject);
    }
}
