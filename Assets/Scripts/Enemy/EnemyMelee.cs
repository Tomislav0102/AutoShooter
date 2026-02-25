using System;
using UnityEngine;

public class EnemyMelee : EnemyCombat
{
    LayerMask _mask;

    public override void InitializeMe(Enemy en)
    {
        base.InitializeMe(en);
        _mask = enemy.isPlayerSummon ? gm.layEnemies : gm.layPlayer;
    }

    protected override void Attack()
    {
        base.Attack();
        
        Vector3 position = transform.position + attackRange * 0.5f * transform.forward + Vector3.up;
        float radius = attackRange * 0.5f;
        
        Collider[] colliders = Physics.OverlapSphere(position, radius, _mask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out ITakeDamage takeDamage))
            {
                takeDamage.TakeDamage(damage);
            }
        }
    }
    

}
