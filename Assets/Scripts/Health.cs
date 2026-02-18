using UnityEngine;
using UnityEngine.Serialization;


public class Health: EventBus, ITakeDamage
{
    [SerializeField] SpriteRenderer healthBarRenderer;
    [SerializeField] int healthMax;
    float HealthCurrent
    {
        get => _healthCurrent;
        set
        {
            _healthCurrent = value;
            healthBarRenderer.color = Color.Lerp(Color.red, Color.green, value / healthMax);
            healthBarRenderer.transform.localScale = new Vector3(_healthCurrent / healthMax, healthBarRenderer.transform.localScale.y, 1);
        }
    }
    float _healthCurrent;

    protected override void Awake()
    {
        base.Awake();
        HealthCurrent = healthMax;
    }

    public void TakeDamage(float damageTaken)
    {
        HealthCurrent -= damageTaken;
        if (HealthCurrent <= 0) Death();
    }
    
    protected virtual void Death()
    {
       
    }

}
