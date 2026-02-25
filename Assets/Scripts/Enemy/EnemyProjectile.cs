using System;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(myData.damage, myData.attacker.MyTransform);
           // myData.onHit?.Invoke($"enemy hits {other.name} for {myData.damage} damage");
        }
        Destroy(gameObject);
    }
}
