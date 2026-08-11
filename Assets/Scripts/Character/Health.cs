using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;


public class Health: MonoBehaviour, IIniBrain
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _healthBar = Instantiate(Ga.me.healthBarPrefab, Ga.me.barContainer).GetComponent<Image>();
            _healthMax = value.character.GetStat(Stats.Health);
            _healthBarTransform = _healthBar.transform;
            _shieldBar = _healthBarTransform.GetChild(0).GetComponent<Image>();
            HealthCurrent = _healthMax;
            _dictPsElements = new Dictionary<Element, ParticleSystem>();
            for (int i = 0; i < psElements.Length; i++)
            {
                _dictPsElements.Add((Element)i, psElements[i]);
            }
            _dictPsStatus = new Dictionary<Status.Effect, ParticleSystem>();
            for (int i = 0; i < psStatus.Length; i++)
            {
                _dictPsStatus.Add((Status.Effect)i, psStatus[i]);
            }
            _screenCenter = new Vector3(Screen.width, Screen.height, 0) * 0.5f;
            _pointer = Instantiate(Ga.me.offScreenPointerPrefab, Ga.me.parPointers);
            _pointerImage = _pointer.GetComponent<Image>();
            
        }
    }
    Brain _br;

    Vector3 _screenCenter; 
    RectTransform _pointer;
    Image _pointerImage;
    Image _healthBar, _shieldBar;
    Transform _healthBarTransform;
    Vector3 _offset = new Vector3(0, 2, 0);
    float _timerRegenerate;
    float HealthCurrent
    {
        get => _healthCurrent;
        set
        {
            _healthCurrent = value;
            if (_healthCurrent > _healthMax)  _healthCurrent = _healthMax;
            _healthBar.color = Color.Lerp(Color.red, Color.green, value / _healthMax);
            _healthBar.fillAmount = _healthCurrent / _healthMax;
        }
    }
    [ShowInInspector, ReadOnly] float _healthCurrent;
    [ShowInInspector, ReadOnly] float _healthMax;
    public bool IsAtFullHealth() => HealthCurrent >= _healthMax;
    float ShieldCurrent
    {
        get => _shieldCurrent;
        set
        {
            if (Mathf.Approximately(_shieldMax, 0f)) return;
            _shieldCurrent = value;
            _shieldCurrent = Mathf.Clamp(_shieldCurrent, 0, _shieldMax);
            _shieldBar.fillAmount = _shieldCurrent / _shieldMax;
        }
    }
    [ShowInInspector, ReadOnly] float _shieldCurrent;
    [ShowInInspector, ReadOnly] float _shieldMax;
    bool IsAtFullShield() => Mathf.Approximately(ShieldCurrent, _shieldMax);
    float _timerShield;
    const int CONST_ShieldWaitTime = 3;
    const int CONST_ShieldRegenAmount = 100;
    
    [Title("Particles")]
    [SerializeField] ParticleSystem[] psElements;
    [SerializeField] ParticleSystem[] psStatus;
    Dictionary<Element, ParticleSystem> _dictPsElements;
    Dictionary<Status.Effect, ParticleSystem> _dictPsStatus;
    [SerializeField] ParticleSystem psHeal, psBleed, psStun, psRoot, psConfuse, psBlind, psCharm;

    public void TakeDamage(PassDataContainer pd)
    {
        FloatingText ft = Instantiate(Ga.me.floatingTextPrefab, Br.myTransform.position, Quaternion.identity, Ga.me.floatingContainer);

        if (pd.canBeBlocked)
        {
            Br.combat.CheckBlock(out bool blocked, pd.myBrain);
            if (blocked)
            {
                ft.SpawnMe("Blocked!", Color.gold);
                return;
            }
            Br.combat.CheckDodge(out bool dodged);
            if (dodged)
            {
                ft.SpawnMe("Dodged!", Color.moccasin);
                return;
            }
        }
        
        foreach (PassData item in pd.data)
        {
            switch (item)
            {
                case PassDataDamage dam:
                    float totalDamage = 0f;
                    for (int i = 0; i < dam.pair.Length(); i++)
                    {
                        float val = dam.pair.GetValue(i);
                        switch (val)
                        {
                            case < 0:
                                psHeal.Play();
                                break;
                            case > 0:
                            {
                                ParticleSystem ps = _dictPsElements[dam.pair.GetKey(i)];
                                if (ps != null) ps.Play();
                                break;
                            }
                        }
                        totalDamage += val;
                    }

                    float shield = ShieldCurrent;
                    ShieldCurrent -= totalDamage;
                    if (ShieldCurrent <= 0)
                    {
                        HealthCurrent -= (totalDamage - shield);
                        if (HealthCurrent <= 0)
                        {
                            if (pd.myBrain != null) pd.myBrain.combat.CombatEventRegistered(CombatEvent.Kill, Br);
                            Death();
                            return;
                        }
                    }
                    ft.SpawnMe(dam.pair);
                    break;

                case PassDataManaShield manaShield:
                    SetShield(manaShield.manaShieldPoints);
                    break;
                
                case PassDataKnockBack knockBack:
                    Vector3 dirKnockback;
                    if (knockBack.knockBackDirection.Equals(Vector2.zero)) dirKnockback = Utils.Direction(pd.myBrain.myTransform.position, Br.myTransform.position);
                    else dirKnockback = Utils.MakeV3(knockBack.knockBackDirection);
                    Br.loco.PushMe(dirKnockback, Loco.MoveOverrideType.KnockBack, knockBack.knockBackPower);
                    break;
                
                case PassDataMagnet magnet:
                    Vector3 dirMagnet = Vector3.zero;
                    Br.loco.PushMe(dirMagnet, Loco.MoveOverrideType.Magnet, magnet.magnetPower);
                    break;
            }

        }

        _timerRegenerate = _timerShield = 0f;
        Br.combat.CombatEventRegistered(CombatEvent.GetHit, pd.myBrain);
        Br.loco.Hit();
        if (pd.myBrain == null) return;
        if (Br.myTransform == Ga.me.team.playerTransform) return;
        if (Br.combat.MyTarget == null)
        {
            print("UnderAttack");
            Br.combat.MyTarget = pd.myBrain.myTransform;
        }
    }

    public void SetShield(float value)
    {
        _shieldMax = value;
        ShieldCurrent = _shieldMax;
    }
    void Update()
    {
        if (!IsAtFullHealth())
        {
            _timerRegenerate += Time.deltaTime;
            if (_timerRegenerate >= 1f)
            {
                _timerRegenerate = 0f;
                HealthCurrent += Br.character.GetStat(Stats.RegenerationRate) * 0.01f;
            }
        }
        else _timerRegenerate = 0f;

        if (!IsAtFullShield())
        {
            _timerShield += Time.deltaTime;
            if (_timerShield >= CONST_ShieldWaitTime)
            {
                ShieldCurrent += CONST_ShieldRegenAmount * Time.deltaTime;
            }
        }
        else _timerShield = 0f;
    }

    void LateUpdate()
    {
        UIdisplay();
        return;
        
        void UIdisplay()
        {
            Vector3 screenPos = Ga.me.camRig.cam.WorldToScreenPoint(Br.myTransform.position + _offset);
            _healthBarTransform.position = screenPos;
            bool isBehind = Vector3.Dot(Ga.me.camRig.camTransform.forward, Br.myTransform.position - Ga.me.camRig.camTransform.position) < 0;
            if (isBehind)  screenPos = _screenCenter - (screenPos - _screenCenter).normalized * Screen.width;
            int offset = 50;
            bool isOffScreen = screenPos.x > Screen.width + offset || screenPos.x + offset < 0 ||
                               screenPos.y > Screen.height + offset || screenPos.y + offset < 0 ||
                               isBehind;
    
            if (isOffScreen)
            {
                _pointerImage.enabled = true;
    
                screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
                screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height);
    
                RectTransform canvasRect = _pointer.parent as RectTransform;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos);
                _pointer.anchoredPosition = localPos;
    
                Vector3 angleDir = screenPos - _screenCenter;
                if (isBehind) angleDir = _screenCenter - screenPos;
                float angle = Mathf.Atan2(angleDir.y, angleDir.x) * Mathf.Rad2Deg;
                _pointer.localRotation = Quaternion.Euler(0, 0, angle - 90f);
            }
            else
            {
                _pointerImage.enabled = false;
            }
        }
    }



    void Death()
    {
        Quaternion rot  = Quaternion.LookRotation(Br.myTransform.forward) * Quaternion.Euler(new Vector3(-90f, 0f, 0f));
        ParticleSystem ps = Instantiate(Ga.me.psDeath, Br.myTransform.position, rot,Ga.me.transform);
        ps.Play();
        EventBus.OnCharDeath?.Invoke(Br);
        Destroy(_healthBar.gameObject);
        Destroy(_pointer.gameObject);
        Destroy(Br.gameObject);
    }


}