using System;
using UnityEngine;

public class EnemyMelee : EnemyCombat
{

    protected override void Attack()
    {
        base.Attack();
        
        Vector3 position = transform.position + range * 0.5f * transform.forward + Vector3.up;
        float radius = range * 0.5f;
        
        Collider[] colliders = Physics.OverlapSphere(position, radius, gm.layPlayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out ITakeDamage takeDamage))
            {
                takeDamage.TakeDamage(damage);
            }
        }
    }
    

}
