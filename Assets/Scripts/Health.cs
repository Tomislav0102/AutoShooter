using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;


public class Health: EventBus, ITakeDamage, IInit
{
    Brain _brain;
    Image _healthBar;
    Vector3 _offset = new Vector3(0, 2, 0);
    [SerializeField] int healthMax;
    float HealthCurrent
    {
        get => _healthCurrent;
        set
        {
            _healthCurrent = value;
            _healthBar.color = Color.Lerp(Color.red, Color.green, value / healthMax);
            _healthBar.transform.localScale = new Vector3(_healthCurrent / healthMax, 1, 1);
        }
    }
    float _healthCurrent;
    
    public void Initialize(Brain brain)
    {
        _brain = brain;
        _healthBar = Instantiate(gm.healthBarPrefab, gm.barContainer).GetComponent<Image>();
        HealthCurrent = healthMax;
        IsReady = true;
    }

    public bool IsReady { get; set; }

    public void TakeDamage(float damageTaken, Transform attacker = null)
    {
        HealthCurrent -= damageTaken;
        FloatingText ft = Instantiate(gm.floatingTextPrefab, gm.floatingContainer);
        ft.SpawnMe(transform, _offset.y, damageTaken.ToString("0"));

        if (HealthCurrent <= 0)
        {
            Death();
            return;
        }
        if (attacker == null) return;
        if (_brain.MyTransform == gm.playerTransform) return;

        if (_brain.MyTarget == null)
        {
            print("UnderAttack");
            _brain.MyTarget = attacker;
        }
    }

    void LateUpdate()
    {
        Vector3 screenPos = gm.cam.WorldToScreenPoint(transform.position + _offset);
        _healthBar.transform.position = screenPos;
    }

    void Death()
    {
       EventBus.OnCharDeath?.Invoke(_brain.MyTransform);
    }

}
