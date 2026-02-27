using UnityEngine;

public class EnemyMarksman : EnemyRanged
{
    [SerializeField] Transform spawnPoint;
    
    protected override void Attack()
    {
        base.Attack();
        float dist = Vector2.Distance(Utils.MakeV2(transform.position), Utils.MakeV2(enemy.MyTarget.position));
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