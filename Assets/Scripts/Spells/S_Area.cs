using UnityEngine;

/// <summary>
/// area effect that last a certain time (DOT)
/// </summary>
public class S_Area : Spell
{
    float _timer = Mathf.Infinity;
    public override void InitializeMe(Brain brain)
    {
        base.InitializeMe(brain);
        myCollider.enabled = true;
    }
    protected override void Update()
    {
        base.Update();
        _timer += Time.deltaTime;
        if (_timer >= 1f)
        {
            _timer = 0f;
            foreach (Collider item in collidersDetected)
            {
                if (item.TryGetComponent(out ITakeDamage takeDamage))
                {
                    takeDamage.TakeDamage(dam);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (lifeTime == 0) return;
        collidersDetected.Add(other);
    }
    
    void OnTriggerExit(Collider other)
    {
        if (lifeTime == 0) return;
        collidersDetected.Remove(other);
    }

}
