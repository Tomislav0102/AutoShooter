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
    // [SerializeField] TextMeshProUGUI myText;
    // Color _startColor = Color.white;
    // Color _endColor = Color.clear;
    
    [SerializeField] RectTransform myTransform;
    [SerializeField] TextMeshProUGUI[] myTexts;
    bool _initialized;
    float _lifeTime = 3f;
    float _timer;
    Vector3 _startPosition;
    float _startingOffsetY;
    int _widthSingle = 65;
    
    
    public void SpawnMe(InjectHealth dam, Vector3 spawnPos, float offsetY = 2f)
    {
        string textToDisplay = String.Empty;
        foreach (KeyValuePair<Element, float> item in dam.damage)
        {
            if (item.Value >= 0) 
            {
                textToDisplay = item.Value.ToString();
            }
            else // healing
            {
                textToDisplay =(-item.Value).ToString();
            }

            int index = (int)item.Key;
            myTexts[index].enabled = true;
            myTexts[index].text = textToDisplay;
        }
        
       // GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
        GetComponent<RectTransform>().sizeDelta = new Vector2(_widthSingle * dam.damage.Count, 0);
        _startPosition = spawnPos; 
        _startingOffsetY = offsetY;
        
        _initialized = true;
        
        // string textToDisplay = String.Empty;
        // foreach (KeyValuePair<Element, float> item in dam.damage)
        // {
        //     if (item.Value >= 0) 
        //     {
        //         _startColor = Ga.me.gameData.GetElement(item.Key).col;
        //         textToDisplay += $" <sprite index={(int)item.Key}> {item.Value} ";
        //     }
        //     else // healing
        //     {
        //         _startColor = Ga.me.gameData.colHeal;
        //         textToDisplay+=$" {-item.Value} ";
        //     }
        // }
        // myText.text = textToDisplay;
        //
        // myText.color = _startColor;
        // _startPosition = spawnPos; 
        // _startingOffsetY = offsetY;
        //
        // _initialized = true;
    }
    
    void LateUpdate()
    {
        if (!_initialized) return;
        
        _timer += Time.deltaTime;
        float moveY = _startingOffsetY + _timer * 5f;
        Vector3 targetPos = new Vector3(_startPosition.x, moveY, _startPosition.z);
        myTransform.position = Ga.me.cam.WorldToScreenPoint(targetPos);
      //  myText.color = Color.Lerp(_startColor, _endColor, _timer / _lifeTime);
        if (_timer >= _lifeTime) EndMe();
    }

    void EndMe()
    {
        _initialized = false;
        
        Destroy(gameObject);
    }
}
