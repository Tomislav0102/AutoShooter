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
    }

    void Death()
    {
        Quaternion rot  = Quaternion.LookRotation(Br.myTransform.forward) * Quaternion.Euler(new Vector3(-90f, 0f, 0f));
        ParticleSystem ps = Instantiate(Ga.me.psDeath, Br.myTransform.position, rot,Ga.me.transform);
        ps.Play();
        EventBus.OnCharDeath?.Invoke(Br);
        Destroy(_healthBar.gameObject);
        Destroy(Br.gameObject);
    }


}