using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UltimateUi : MonoBehaviour
{
    [SerializeField] GameObject parMain;
    [SerializeField] Image ultimateFill;
    [SerializeField] Button btnUltimate;
    float _cooldownTime;
    float _timer;
    bool _initialized;

    public void SetMeUp(float cooldownTime)
    {
        if (!_initialized)
        {
            ultimateFill.fillAmount = 1f;
            btnUltimate.enabled = true;
            _timer = 0f;
            parMain.SetActive(true);
        }
        _initialized = true;
        _cooldownTime = cooldownTime;
    }

    void Start()
    {
        parMain.SetActive(false);
        Ga.OnUltimateActivated += CallEv_OnUltimateActivated;
    }
    void OnDestroy()
    {
        Ga.OnUltimateActivated -= CallEv_OnUltimateActivated;
    }

    public void BtnClicked()
    {
        Ga.OnUltimateActivated?.Invoke();
    }

    void CallEv_OnUltimateActivated()
    {
        StartCoroutine(coolDown());
        return;
        
        IEnumerator coolDown()
        {
            btnUltimate.enabled = false;
            ultimateFill.fillAmount = 0f;

            while (_timer < _cooldownTime)
            {
                _timer += Time.deltaTime;
                ultimateFill.fillAmount = _timer / _cooldownTime;
                yield return Ga.me.waitEndFrame;
            }
            ultimateFill.fillAmount = 1f;
            btnUltimate.enabled = true;
            _timer = 0f;
        }
        
    }


    
}
