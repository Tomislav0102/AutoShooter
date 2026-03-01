using System;
using UnityEngine;

public class EnemyMelee : EnemyCombat
{
    AttackOverlapSphere _sphere;

    public override void InitializeMe(Enemy en)
    {
        base.InitializeMe(en);
        _sphere = new AttackOverlapSphere(transform, attackRange, enemy.isPlayerSummon ? gm.layEnemies : gm.layPlayer);
    }

    protected override void Attack()
    {
        base.Attack();
        _sphere.Attack(damage);
    }
    

}

public class AttackOverlapSphere
{
    Transform _myTransform;
    float _attackRange;
    LayerMask _mask;

    public AttackOverlapSphere(Transform myTransform, float attackRange, LayerMask mask)
    {
        _myTransform = myTransform;
        _attackRange = attackRange;
        _mask = mask;
    }
    
    public void Attack(float damage)
    {
        Vector3 position = _myTransform.position + _attackRange * 0.5f * _myTransform.forward + Vector3.up;
        float radius = _attackRange * 0.5f;
        
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
