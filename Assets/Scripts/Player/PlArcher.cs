using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlArcher : PlProfession
{
    [SerializeField] float bulletSpeed;
    [SerializeField] bool front;
    [SerializeField] bool diagonal, side, back, homing;
    [SerializeField] int parallel, followUp, ricochet, pierce, bounce;
    const float CONST_FollowUp = 0.1f;
    const float CONST_HorGapBetweenProjectiles = 0.3f;


    
    public override void AttackAnimEvent()
    {
        base.AttackAnimEvent();
        if (!isAttacking) return;
        Shoot();
        StartCoroutine(ShootFollowUp());
    }
    
    IEnumerator ShootFollowUp()
    {
        for (int i = 0; i < followUp; i++)
        {
            yield return new WaitForSeconds(CONST_FollowUp);
            Shoot();
        }
    }

    void Shoot()
    {
        if (front)
        {
            SpawnProjectile(0f);
        }
        if (back)
        {
            SpawnProjectile(180f);
        }
        if (diagonal)
        {
            for (int i = 0; i < 2; i++)
            {
                SpawnProjectile(45 * (i * 2 - 1));
            }
        }
        if (side)
        {
            for (int i = 0; i < 2; i++)
            {
                SpawnProjectile(90 * (i * 2 - 1));
            }
        }
        if (homing)
        {
            Transform closest = Utils.ClosestTransform(transform.position, gm.allEnemies);
            if (closest != null)
            {
                Vector3 dir = (closest.position - transform.position).normalized;
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                SpawnProjectile(angle);
            }
        }

        void SpawnProjectile(float rotation)
        {
            for (int i = 0; i < parallel + 1; i++)
            {
                float xOffset = i * CONST_HorGapBetweenProjectiles;
                Projectile projectile = Instantiate<Projectile>(gm.projectilePrefab, transform.position + Vector3.up, Quaternion.identity);
                projectile.transform.Rotate(rotation * Vector3.up);
                projectile.transform.Translate(xOffset * Vector3.right, Space.Self);
                float width = (parallel + 1) * CONST_HorGapBetweenProjectiles;
                projectile.transform.Translate((width - CONST_HorGapBetweenProjectiles) * 0.5f * Vector3.left, Space.Self);

                ProjectilePassData passData = new ProjectilePassData((string message) => { print(message); }, parentCharacter, damage, bulletSpeed, ricochet, pierce, bounce);
                projectile.InitializeMe(passData);
            }

        }
    }
}
