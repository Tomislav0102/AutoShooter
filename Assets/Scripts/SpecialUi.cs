using System;
using System.Collections;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;

public class SpecialUi : MonoBehaviour
{
    [SerializeField] Image specialBackground;
    [SerializeField] Button btnSpecial;
    float _cooldownTime;
    float _timerSpecial;

    public void InitializeMe(float cooldownTime)
    {
        specialBackground.enabled = true;
        specialBackground.fillAmount = 1f;
        btnSpecial.enabled = true;
        btnSpecial.GetComponent<Image>().enabled = true;
        _cooldownTime = cooldownTime;
        _timerSpecial = 0f;
    }

    void OnEnable()
    {
        EventBus.OnSpecialActivated += CallEv_OnSpecialActivated;
    }
    void OnDisable()
    {
        EventBus.OnSpecialActivated -= CallEv_OnSpecialActivated;
    }

    public void BtnClicked()
    {
        EventBus.OnSpecialActivated?.Invoke();
    }

    void CallEv_OnSpecialActivated()
    {
        StartCoroutine(CoolDown());
        IEnumerator CoolDown()
        {
            btnSpecial.enabled = false;
            specialBackground.fillAmount = 0f;

            while (_timerSpecial < _cooldownTime)
            {
                _timerSpecial += Time.deltaTime;
                specialBackground.fillAmount = _timerSpecial / _cooldownTime;
                yield return null;
            }
            specialBackground.fillAmount = 1f;
            btnSpecial.enabled = true;
            _timerSpecial = 0f;
        }
        
    }


    
}
