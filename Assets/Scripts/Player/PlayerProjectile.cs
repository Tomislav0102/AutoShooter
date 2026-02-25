using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(myData.damage);
            if (myData.ricochet > 0)
            {
                myData.ricochet--;
                
                float range = 1f;
                Collider[] colliders = Physics.OverlapSphere(transform.position, range);
                HashSet<Transform> allEnemies = new HashSet<Transform>();
                foreach (Collider item in colliders)
                {
                    if (item != other.collider && item.TryGetComponent<Enemy>(out Enemy en)) allEnemies.Add(item.transform);
                }
                Transform closestEnemy = Utils.ClosestTransform(transform.position, allEnemies);
                if (closestEnemy != null)
                {
                    Vector3 dir = closestEnemy.transform.position - transform.position;
                    transform.forward = dir.normalized;
                    SetSpeed();
                }
                else SetPierce();
            }
            else SetPierce();
            
            void SetPierce()
            {
                if (myData.pierce > 0)
                {
                    myData.pierce--;
                }
                else Destroy(gameObject);
            }
        }
        
        if (other.collider.TryGetComponent(out IObstacle iObstacle))
        {
            if (myData.bounce > 0)
            {
                myData.bounce--;
                Vector3 dir = Vector3.Reflect(transform.forward, other.GetContact(0).normal);
                transform.forward = dir.normalized;
                SetSpeed();
            }
            else Destroy(gameObject);
        }
    }


}