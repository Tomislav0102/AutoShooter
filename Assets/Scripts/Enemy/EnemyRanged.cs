using UnityEngine;

public class EnemyRanged : EnemyCombat
{
    [SerializeField] EnemyProjectile projectilePrefab;
    [SerializeField] float projectileSpeed;
    ICharacter _parentCharacter;

    public override void InitializeMe(Enemy en)
    {
        base.InitializeMe(en);
        _parentCharacter = en.GetComponent<ICharacter>();
    }

    protected void SpawnProjectile(Transform spawnPointTransform)
    {
        EnemyProjectile projectile = Instantiate(projectilePrefab, spawnPointTransform.position, spawnPointTransform.rotation);
        ProjectilePassData passData = new ProjectilePassData((string st) =>
        {
            print(st);
        }, _parentCharacter, damage, projectileSpeed);
        projectile.InitializeMe(passData);
    }

}
