using UnityEngine;

public class EnemyRanged : EnemyCombat
{
    [SerializeField] EnemyProjectile projectilePrefab;
    [SerializeField] float projectileSpeed;
    
    
    protected void SpawnProjectile(Transform spawnPointTransform)
    {
        EnemyProjectile projectile = Instantiate(projectilePrefab, spawnPointTransform.position, spawnPointTransform.rotation);
        ProjectilePassData passData = new ProjectilePassData((string st) =>
        {
            print(st);
        }, damage, projectileSpeed);
        projectile.InitializeMe(passData);
    }

}
