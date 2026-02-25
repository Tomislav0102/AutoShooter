using UnityEngine;

public class EnemyMarksman : EnemyRanged
{
    [SerializeField] Transform spawnPoint;
    
    protected override void Attack()
    {
        base.Attack();
        float dist = Vector2.Distance(Utils.From3d(transform.position), Utils.From3d(enemy.MyTarget.position));
        if (dist <= attackRange)
        {
            SpawnProjectile(spawnPoint);
        }
        else
        {
            enemy.moveCurrent = EnMovement.Chase;
        }
    }

}