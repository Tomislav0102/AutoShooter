using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// area effect that last a certain time (DOT)
/// </summary>
public class S_Area : Spell
{

    float _timer;
    public override void InitializeMe(Brain brain)
    {
        base.InitializeMe(brain);
        comp.mySphereCollider.enabled = true;
        _timer = Mathf.Infinity;
        if (comp.myMesh != null && comp.myMesh.TryGetComponent(out ParticleSystem ps))
        {
            var emisson = ps.emission;
            emisson.rateOverTime = areaOfEffect * 5;
            var shape = ps.shape;
            shape.radius = areaOfEffect * 0.5f;
        }
        
            
        
    }
    protected override void Update()
    {
        base.Update();
        _timer += Time.deltaTime;
        if (collidersDetected.Count == 0) return;
        if (_timer >= 1f)
        {
            _timer = 0f;
            foreach (Collider item in collidersDetected)
            {
                if (item.TryGetComponent(out ITakeDamage takeDamage) && Utils.TargetFaction(myFaction) == takeDamage.Br.faction)
                {
                    takeDamage.TakeDamage(damData);
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
