using System;
using UnityEngine;

public class E_Projectile : Projectile
{
    
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(myData.damage, myData.attacker.loco.myTransform);
           // myData.onHit?.Invoke($"enemy hits {other.name} for {myData.damage} damage");
        }
        Destroy(gameObject);
    }
}
