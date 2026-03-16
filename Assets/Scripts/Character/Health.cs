using System;
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
            HealthCurrent = healthMax;
            _healthBarTransform = _healthBar.transform;
            IsReady = true;
        }
    }
    Brain _br;

    Image _healthBar;
    Transform _healthBarTransform;
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
    public bool IsReady { get; set; }



    public void TakeDamage(DamageData dam)
    {
        HealthCurrent -= dam.damage;
        
        FloatingText ft = Instantiate(Ga.me.floatingTextPrefab, Ga.me.floatingContainer);
        DamageData damToFloat = new DamageData()
        {
            damage = dam.damage,
            attacker = Br.loco.myTransform,
            element = dam.element,
        };
        ft.SpawnMe(damToFloat);
        
        if (HealthCurrent <= 0)
        {
            Death();
            return;
        }
        if (dam.attacker == null) return;
        if (Br.loco.myTransform == Ga.me.playerTransform) return;
        
        Br.loco.Hit();
        Br.loco.KnockBackMe(dam.attacker.position);
        if (Br.combat.MyTarget == null)
        {
            print("UnderAttack");
            Br.combat.MyTarget = dam.attacker;
        }
    }

    void LateUpdate()
    {
        if (!IsReady) return;
        Vector3 screenPos = Ga.me.cam.WorldToScreenPoint(Br.loco.myTransform.position + _offset);
        _healthBarTransform.position = screenPos;
    }

    void Death()
    {
       Destroy(_healthBar.gameObject);
       EventBus.OnCharDeath?.Invoke(Br.loco.myTransform);
    }

}

[System.Serializable]
public struct DamageData
{
    public Transform attacker;
    public float damage;
    public Element element;
}