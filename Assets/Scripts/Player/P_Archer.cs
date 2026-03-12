using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class P_Archer : PlayerCombat
{
    [SerializeField] float bulletSpeed;
    [SerializeField] bool forward;
    [SerializeField] bool front;
    [SerializeField] bool diagonal, side, back;
    [SerializeField][Range(0, 3)] int parallel, followUp, ricochet, pierce, bounce;
    const float CONST_FollowUp = 0.1f;
    const float CONST_HorGapBetweenProjectiles = 0.3f;


    
    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        // GameObject go = Instantiate(GameManager.Instance.spellManager.projectile, 
        //     Utils.FrontSpawnPos(br.loco.myTransform, GameManager.Instance.spellManager.melee.GetComponent<S_Area>().myData.areaOfEffect), 
        //     br.loco.myTransform.rotation);
        // S_Ballistic ran = go.GetComponent<S_Ballistic>();
        // ran.InitializeMe(() =>
        // {
        //     print("hit");
        // });
        // ran.myData.layTarget = GameManager.Instance.layEnemies;
        //
        // Shoot();
        // if (followUp > 0) StartCoroutine(ShootFollowUp());
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
        if (forward)
        {
            Vector3 fw = transform.forward;
            float angle = Mathf.Atan2(fw.x, fw.z) * Mathf.Rad2Deg;
            SpawnProjectile(angle);
        }

        void SpawnProjectile(float rotation)
        {
            // Vector3 rot = rotation * Vector3.up;
            // for (int i = 0; i < parallel + 1; i++)
            // {
            //     float xOffset = i * CONST_HorGapBetweenProjectiles;
            //     Projectile projectile = Instantiate<Projectile>(GameManager.Instance.projectilePrefabPlayer, transform.position + Vector3.up, Quaternion.identity);
            //     projectile.transform.Rotate(rot);
            //     projectile.transform.Translate(xOffset * Vector3.right, Space.Self);
            //     float width = (parallel + 1) * CONST_HorGapBetweenProjectiles;
            //     projectile.transform.Translate((width - CONST_HorGapBetweenProjectiles) * 0.5f * Vector3.left, Space.Self);
            //
            //     ProjectilePassData passData = new ProjectilePassData((string message) => { print(message); }, br, weapons[1].myData.damage, bulletSpeed, ricochet, pierce, bounce);
            //     projectile.InitializeMe(passData);
            // }
        }
    }
}
