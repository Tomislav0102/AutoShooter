using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    protected override void Awake()
    {
        base.Awake();
        gm.enemyManager.allEnemies.Add(transform);
    }

    protected override void Death()
    {
        base.Death();
        Destroy(gameObject);
        HashSet<Transform> all = GameManager.Instance.enemyManager.allEnemies;
        if (all.Contains(transform)) all.Remove(transform);
    }
}
