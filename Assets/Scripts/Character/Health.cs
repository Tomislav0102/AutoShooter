using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;


public class Health: EventBus, IInit
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _healthBar = Instantiate(Ga.me.healthBarPrefab, Ga.me.barContainer).GetComponent<Image>();
            _healthMax = value.myChar.GetStat(Stats.Health);
            _healthBarTransform = _healthBar.transform;
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
            _camTransform = Ga.me.cam.transform;
            _screenCenter = new Vector3(Screen.width, Screen.height, 0) * 0.5f;
            _pointer = Instantiate(Ga.me.offScreenPointerPrefab, Ga.me.parPointers);
            _pointerImage = _pointer.GetComponent<Image>();
            
            IsInitialized = true;
        }
    }
    Brain _br;

    Transform _camTransform;
    Vector3 _screenCenter; 
    RectTransform _pointer;
    Image _pointerImage;
    Image _healthBar;
    Transform _healthBarTransform;
    Vector3 _offset = new Vector3(0, 2, 0);
    
    float HealthCurrent
    {
        get => _healthCurrent;
        set
        {
            _healthCurrent = value;
            if (_healthCurrent > _healthMax)  _healthCurrent = _healthMax;
            _healthBar.color = Color.Lerp(Color.red, Color.green, value / _healthMax);
            _healthBarTransform.localScale = new Vector3(_healthCurrent / _healthMax, 1, 1);
        }
    }
    [ShowInInspector, ReadOnly] float _healthCurrent;
    [ShowInInspector, ReadOnly] float _healthMax;
    public bool IsAtFullHealth() => HealthCurrent >= _healthMax;
    public bool IsInitialized { get; set; }
    
    [Title("Particles")]
    [SerializeField] ParticleSystem[] psElements;
    [SerializeField] ParticleSystem[] psStatus;
    Dictionary<Element, ParticleSystem> _dictPsElements;
    Dictionary<Status.Effect, ParticleSystem> _dictPsStatus;
    [SerializeField] ParticleSystem psHeal, psBleed, psStun, psRoot, psConfuse, psBlind, psCharm;

    public void TakeDamage(InjectHealth dam)
    {
        FloatingText ft = Instantiate(Ga.me.floatingTextPrefab, Br.myTransform.position, Quaternion.identity, Ga.me.floatingContainer);
        if (dam.canBeBlocked)
        {
            Br.combat.CheckBlock(out bool blocked, dam.myBrain);
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

        float totalDamage = 0f;
        foreach (KeyValuePair<Element, float> item in dam.damage)
        {
            totalDamage += item.Value;
            if (item.Value < 0)
            {
                psHeal.Play();
            }
            else
            {
                ParticleSystem ps = _dictPsElements[item.Key];
                if (ps != null) ps.Play();
            }
        }
        Br.combat.CombatEventRegistered(CombatEvent.GetHit, dam.myBrain);
       // Instantiate(Ga.me.dropPrefab, Br.myTransform.position + Vector3.up, Quaternion.identity, Ga.me.transform);

        foreach (KeyValuePair<string, string> item in dam.tags)
        {
            switch (item.Key)
            {
                case InjectHealth.TagExecutioner:
                    float chance = float.Parse(item.Value);
                    float currentHpRatio = HealthCurrent / _healthMax;
                    if (chance <= currentHpRatio)
                    {
                        HealthCurrent = 0;
                    }
                    break;
                    case InjectHealth.TagStatusBleed:
                        //apply bleed
                        break;
            }
        }
        ft.SpawnMe(dam);
        
        HealthCurrent -= totalDamage;
        if (HealthCurrent <= 0)
        {
            if (dam.myBrain != null) dam.myBrain.combat.CombatEventRegistered(CombatEvent.Kill, Br);
            Death();
            return;
        }
        
        Br.loco.Hit();
        if (dam.myBrain == null) return;
        Br.loco.KnockBack((Br.myTransform.position - dam.myBrain.myTransform.position).normalized, dam.knockBack);
        if (Br.myTransform == Ga.me.team.playerTransform) return;
        
        if (Br.combat.MyTarget == null)
        {
            print("UnderAttack");
            Br.combat.MyTarget = dam.myBrain.myTransform;
        }
    }

    void LateUpdate()
    {
        if (!IsInitialized) return;
        Vector3 screenPos = Ga.me.cam.WorldToScreenPoint(Br.myTransform.position + _offset);
        _healthBarTransform.position = screenPos;
        
        bool isBehind = Vector3.Dot(_camTransform.forward, Br.myTransform.position - _camTransform.position) < 0;
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