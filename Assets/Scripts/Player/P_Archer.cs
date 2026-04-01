using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class P_Archer : PlayerCombat
{
    [Title("Archer")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] bool forward;
    [SerializeField] bool front;
    [SerializeField] bool diagonal, side, back;
    [SerializeField][Range(0, 3)] int parallel, followUp, ricochet, pierce, bounce;
    const float CONST_FollowUp = 0.1f;
    const float CONST_HorGapBetweenProjectiles = 0.3f;


    
    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        Shoot();
        if (followUp > 0) StartCoroutine(ShootFollowUp());
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
            Vector3 fw = Br.myTransform.forward;
            float angle = Mathf.Atan2(fw.x, fw.z) * Mathf.Rad2Deg;
            SpawnProjectile(angle);
        }

        void SpawnProjectile(float rotation)
        {
            Vector3 rot = rotation * Vector3.up;
            for (int i = 0; i < parallel + 1; i++)
            {
                float xOffset = i * CONST_HorGapBetweenProjectiles;
                S_Bullet sp = Instantiate(Ga.me.spells.projectilePlayer, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform).GetComponent<S_Bullet>();
                sp.myMesh.position = new Vector3(sp.myMesh.position.x, spawnPoint.position.y, sp.myMesh.position.z);
                sp.myTransform.Rotate(rot);
                sp.myTransform.Translate(xOffset * Vector3.right, Space.Self);
                float width = (parallel + 1) * CONST_HorGapBetweenProjectiles;
                sp.myTransform.Translate((width - CONST_HorGapBetweenProjectiles) * 0.5f * Vector3.left, Space.Self);
                
                sp.ricochet = ricochet;
                sp.pierce = pierce;
                sp.bounce = bounce;
                sp.InitializeMe(Br);
            }

        }
    }
}
