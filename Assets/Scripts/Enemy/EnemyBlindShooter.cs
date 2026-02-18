using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBlindShooter : EnemyRanged
{
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] MultiShot multiShotType;
    int _counter;
    

    protected override void Attack()
    {
        base.Attack();
        switch (multiShotType)
        {
            case MultiShot.AllAtOnce:
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    SpawnProjectile(spawnPoints[i]);
                }
                break;
            
            case MultiShot.Consecutive:
                SpawnProjectile(spawnPoints[_counter]);
                _counter = (1 + _counter) % spawnPoints.Length;
                break;
            
            case MultiShot.Random:
                _counter = Random.Range(0, spawnPoints.Length);
                SpawnProjectile(spawnPoints[_counter]);
                break;
        }

    }
}
