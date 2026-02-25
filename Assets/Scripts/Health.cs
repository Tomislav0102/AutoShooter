using UnityEngine;
using UnityEngine.Serialization;


public class Health: EventBus, ITakeDamage
{
    ICharacter _parentCharacter;
    Enemy _enemy;
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
           // print($"{gameObject.name} is now {value}");
        }
    }
    float _healthCurrent;

    public void InitializeMe(ICharacter character)
    {
        _parentCharacter = character;
        HealthCurrent = healthMax;
    }

    public void TakeDamage(float damageTaken, Transform attacker = null)
    {
        HealthCurrent -= damageTaken;
        if (HealthCurrent <= 0)
        {
            Death();
            return;
        }
        if (attacker == null) return;
        if (_parentCharacter.MyTransform == gm.playerTransform) return;

        if (_parentCharacter.MyTarget == null)
        {
            print("UnderAttack");
            _parentCharacter.MyTarget = attacker;
        }
    }
    
    void Death()
    {
       EventBus.OnCharDeath?.Invoke(_parentCharacter.MyTransform);
    }

}
