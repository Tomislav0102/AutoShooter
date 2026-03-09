using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class E_BlindShooter : EnemyCombat
{
    [SerializeField] int cannons;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] E_Loco.MultiShot multiShotType;
    int _counter;
    

    public override void AE_Attack(int num = 0)
    {
        base.AE_Attack(num);
        Vector3 fw = br.loco.myTransform.forward;
        switch (multiShotType)
        {
            case E_Loco.MultiShot.AllAtOnce:
                for (int i = 0; i < cannons; i++)
                {
                    SpawnProjectile(br.loco.myTransform.position, Quaternion.Euler(fw) * Quaternion.Euler(0, i * (360 / cannons), 0));
                }
                break;
            
            case E_Loco.MultiShot.Consecutive:
                SpawnProjectile(spawnPoints[_counter]);
                _counter = (1 + _counter) % spawnPoints.Length;
                break;
            
            case E_Loco.MultiShot.Random:
                _counter = Random.Range(0, spawnPoints.Length);
                SpawnProjectile(spawnPoints[_counter]);
                break;
        }

    }
    // protected override void Attack()
    // {
    //     base.Attack();
    //     Vector3 fw = transform.forward;
    //     switch (multiShotType)
    //     {
    //         case E_Loco.MultiShot.AllAtOnce:
    //             for (int i = 0; i < spawnPoints.Length; i++)
    //             {
    //                 SpawnProjectile(spawnPoints[i]);
    //             }
    //             break;
    //         
    //         case E_Loco.MultiShot.Consecutive:
    //             SpawnProjectile(spawnPoints[_counter]);
    //             _counter = (1 + _counter) % spawnPoints.Length;
    //             break;
    //         
    //         case E_Loco.MultiShot.Random:
    //             _counter = Random.Range(0, spawnPoints.Length);
    //             SpawnProjectile(spawnPoints[_counter]);
    //             break;
    //     }
    //
    // }
}
