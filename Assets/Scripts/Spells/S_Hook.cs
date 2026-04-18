using UnityEngine;

/// <summary>
/// 1 unit effect that last a certain time (DOT)
/// </summary>
public class S_Hook : Spell
{
    float _timer = float.MaxValue;
    public override void InitializeMe(Brain brain, float dam)
    {
        base.InitializeMe(brain, dam);
        if (anchor == null || anchor.GetComponent<ITakeDamage>() == null) OnEnd();
    }

    protected override void Update()
    {
        base.Update();
        _timer += Time.deltaTime;
        if (_timer > 1f)
        {
            _timer = 0f;
            anchor.GetComponent<ITakeDamage>().TakeDamage(injectHealthData);
            if (lifeTime == 0) OnEnd();
        }
    }
}
