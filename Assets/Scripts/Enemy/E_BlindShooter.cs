using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class E_BlindShooter : EnemyCombat
{
    [SerializeField] int cannons;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] E_Loco.MultiShot multiShotType;
    int _counter;
    

    protected override void Attack()
    {
        base.Attack();
        Vector3 fw = br.MyTransform.forward;
        switch (multiShotType)
        {
            case E_Loco.MultiShot.AllAtOnce:
                for (int i = 0; i < cannons; i++)
                {
                    SpawnProjectile(br.MyTransform.position, Quaternion.Euler(fw) * Quaternion.Euler(0, i * (360 / cannons), 0));
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
