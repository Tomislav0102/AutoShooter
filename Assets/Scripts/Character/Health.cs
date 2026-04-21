using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;


public class Health: EventBus, ITakeDamage, IInit
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
            IsInitialized = true;
        }
    }
    Brain _br;

    Image _healthBar;
    Transform _healthBarTransform;
    Vector3 _offset = new Vector3(0, 2, 0);
    
    float HealthCurrent
    {
        get => _healthCurrent;
        set
        {
           // if (!IsReady) return;
            _healthCurrent = value;
            _healthBar.color = Color.Lerp(Color.red, Color.green, value / _healthMax);
            _healthBarTransform.localScale = new Vector3(_healthCurrent / _healthMax, 1, 1);
        }
    }
    [ShowInInspector, ReadOnly] float _healthCurrent;
    [ShowInInspector, ReadOnly] float _healthMax;
    public bool IsInitialized { get; set; }
    
    [Title("Particles")]
    [SerializeField] ParticleSystem[] psElements;
    [SerializeField] ParticleSystem[] psStatus;
    Dictionary<Element, ParticleSystem> _dictPsElements;
    Dictionary<Status.Effect, ParticleSystem> _dictPsStatus;
    [SerializeField] ParticleSystem psHeal, psBleed, psStun, psRoot, psConfuse, psBlind, psCharm;

    public void TakeDamage(InjectHealth dam)
    {
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
        HealthCurrent -= totalDamage;

        FloatingText ft = Instantiate(Ga.me.floatingTextPrefab, Ga.me.floatingContainer);
        ft.SpawnMe(dam, Br.myTransform.position);
        
        if (HealthCurrent <= 0)
        {
            Death();
            return;
        }
        if (dam.attacker == null) return;
        
        Br.loco.Hit();
        Br.loco.KnockBack((Br.myTransform.position - dam.attacker.position).normalized, dam.knockBack);
        if (dam.canBeBlocked && Br.myTransform == Ga.me.playerTransform)
        {
            PlayerCombat pc = Br.combat as PlayerCombat;
            pc.HealthHitCallback(dam);
            return;
        }
        
        if (Br.combat.MyTarget == null)
        {
            print("UnderAttack");
            Br.combat.MyTarget = dam.attacker;
        }
    }

    void LateUpdate()
    {
        if (!IsInitialized) return;
        Vector3 screenPos = Ga.me.cam.WorldToScreenPoint(Br.myTransform.position + _offset);
        _healthBarTransform.position = screenPos;
    }

    void Death()
    {
        Quaternion rot  = Quaternion.LookRotation(Br.myTransform.forward) * Quaternion.Euler(new Vector3(-90f, 0f, 0f));
        ParticleSystem ps = Instantiate(Ga.me.psDeath, Br.myTransform.position, rot);
        ps.Play();
        Destroy(_healthBar.gameObject);
        EventBus.OnCharDeath?.Invoke(Br.myTransform);
    }


}