using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UltimateUi : MonoBehaviour
{
    [SerializeField] Image ultimateBackground;
    [SerializeField] Button btnUltimate;
    float _cooldownTime;
    float _timer;

    public void InitializeMe(float cooldownTime)
    {
        ultimateBackground.enabled = true;
        ultimateBackground.fillAmount = 1f;
        btnUltimate.enabled = true;
        btnUltimate.GetComponent<Image>().enabled = true;
        _cooldownTime = cooldownTime;
        _timer = 0f;
    }

    void OnEnable()
    {
        Ga.OnUltimateActivated += CallEv_OnUltimateActivated;
    }
    void OnDisable()
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
            ultimateBackground.fillAmount = 0f;

            while (_timer < _cooldownTime)
            {
                _timer += Time.deltaTime;
                ultimateBackground.fillAmount = _timer / _cooldownTime;
                yield return null;
            }
            ultimateBackground.fillAmount = 1f;
            btnUltimate.enabled = true;
            _timer = 0f;
        }
        
    }


    
}
