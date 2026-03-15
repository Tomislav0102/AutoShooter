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
            IsReady = true;
        }
    }
    Brain _br;

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
    public bool IsReady { get; set; }



    public void TakeDamage(DamageData dam)
    {
        HealthCurrent -= dam.damage;
        FloatingText ft = Instantiate(Ga.me.floatingTextPrefab, Ga.me.floatingContainer);
        Color col = Ga.me.gameData.GetElement(dam.element).col;
        ft.SpawnMe(transform, _offset.y, dam.damage.ToString("0"), col);
        Br.onHit?.Invoke();
        if (HealthCurrent <= 0)
        {
            Death();
            return;
        }
        if (dam.attacker == null) return;
        if (Br.loco.myTransform == Ga.me.playerTransform) return;

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
        _healthBar.transform.position = screenPos;
    }

    void Death()
    {
       Destroy(_healthBar.gameObject);
       EventBus.OnCharDeath?.Invoke(Br.loco.myTransform);
    }

}

public struct DamageData
{
    public Transform attacker;
    public float damage;
    public Element element;
}