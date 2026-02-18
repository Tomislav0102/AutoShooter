using System;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    ProjectilePassData _myData;

    public void InitializeMe(ProjectilePassData passData)
    {
        _myData = passData;
        SetSpeed();
        Destroy(gameObject, 10);
    }

    void SetSpeed() => rigid.linearVelocity = _myData.moveSpeed * transform.forward;


    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(_myData.damage);
            _myData.onHit?.Invoke($"enemy hits {other.name} for {_myData.damage} damage");
            Destroy(gameObject);
        }
    }
}
