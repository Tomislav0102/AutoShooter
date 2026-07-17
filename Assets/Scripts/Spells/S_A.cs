using UnityEngine;

public class S_A : Spell
{
    float _timer = Mathf.Infinity;
    bool _oneHit;

    
    protected override void Update()
    {
        base.Update();
        if (!initialized) return;
        if (!main.mainActive) return;
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
      //  if (_timer > Ga.me.gameData.rofSpells)
        {
            _timer = 0f;
            Hit();
        }
    }
    
    protected virtual void Hit() { }

}
