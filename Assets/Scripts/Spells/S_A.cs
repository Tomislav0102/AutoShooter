using UnityEngine;

public class S_A : Spell
{
    float _timer = Mathf.Infinity;
    bool _oneHit;

    
    protected override void Update()
    {
        base.Update();
        if (!main.IsActive) return;
        if (MyPhase != Phase.SpellRuns) return;
        if (_oneHit) return;

        if (rateOfFire == 0)
        {
            Hit();
            _oneHit = true;
            return;
        }
        
        _timer += Time.deltaTime;
        if (_timer > rateOfFire)
        {
            _timer = 0f;
            Hit();
        }
    }
    
    protected virtual void Hit() { }

}
