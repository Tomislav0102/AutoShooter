using UnityEngine;

public class S_Area : Spell
{
    public override void InitializeMe(Brain brain)
    {
        base.InitializeMe(brain);
        myCollider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name);
        if (lifeTime == 0) return;
        if (collidersDetected.Contains(other)) return; //if is not redunant
        collidersDetected.Add(other);
        if (other.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(dam);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (lifeTime == 0) return;
        collidersDetected.Remove(other);
    }

}
