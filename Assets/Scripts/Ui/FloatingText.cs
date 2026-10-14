using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour, IPoolObject
{
    public PoolManager.Phase PoolPhase
    {
        get => _poolPhase; 
        set
        {
            _poolPhase = value;
            switch (_poolPhase)
            {
                case PoolManager.Phase.UnInitialized:
                    break;
                case PoolManager.Phase.Initialized:
                    break;
                case PoolManager.Phase.Running:
                    break;
                case PoolManager.Phase.Ended:
                    for (int i = 0; i < myTexts.Length; i++)
                    {
                        myTexts[i].text = string.Empty;
                    }
                    _timer = 0f;
                    break;
            }
        }
    }
    [ReadOnly, ShowInInspector] PoolManager.Phase _poolPhase;
    Color _endColor = Color.clear;
    
    [SerializeField] RectTransform myTransform;
    [SerializeField] TextMeshProUGUI[] myTexts;
    [SerializeField] HorizontalLayoutGroup myHorizontalLayoutGroup;
    Color[] _startColors;
    const float CONST_LifeTime = 3f;
    float _timer;
    Vector3 _startPosition;
    float _startingOffsetY;
    const int CONST_WidthSingle = 100;
    
    public void StartMe(Vector3 spawnPoint, MyDuo<Element, float> damage, float offsetY = 2f)
    {
        Shared(spawnPoint, offsetY);
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
        
        myTransform.sizeDelta = new Vector2(CONST_WidthSingle * effDamage.Length(), 0);
    }    
    public void StartMe(Vector3 spawnPoint, string st, Color col, float offsetY = 2f)
    {
        Shared(spawnPoint, offsetY);
        myTexts[0].text = st;
        myTexts[0].fontStyle = FontStyles.Bold;
        _startColors[0] = col;
        myTransform.sizeDelta = new Vector2(CONST_WidthSingle, 0);
        
    }
    void Shared(Vector3 spawnPoint, float offsetY)
    {
        _startColors = new Color[myTexts.Length];
        myHorizontalLayoutGroup.childControlWidth = true;
        _startPosition = myTransform.position = spawnPoint; 
        _startingOffsetY = offsetY;
        PoolPhase = PoolManager.Phase.Running;
    }
    
    void LateUpdate()
    {
        switch (PoolPhase)
        {
            case PoolManager.Phase.UnInitialized:
                break;
            case PoolManager.Phase.Initialized:
                break;
            case PoolManager.Phase.Running:
                _timer += Time.deltaTime;
                float moveY = _startingOffsetY + _timer * 5f;
                Vector3 targetPos = new Vector3(_startPosition.x, moveY, _startPosition.z);
                myTransform.position = Ga.me.camRig.cam.WorldToScreenPoint(targetPos);
                for (int i = 0; i < myTexts.Length; i++)
                {
                    TextMeshProUGUI tmp = myTexts[i];
                    float val = _timer / CONST_LifeTime;
                    tmp.color = Color.Lerp(_startColors[i], _endColor, val);
                }
                if (_timer >= CONST_LifeTime) PoolPhase = PoolManager.Phase.Ended;
                break;
            case PoolManager.Phase.Ended:
                break;
        }

        
    }

}
