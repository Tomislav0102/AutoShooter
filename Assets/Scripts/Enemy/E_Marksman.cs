using UnityEngine;

public class E_Marksman : EnemyCombat
{
    [SerializeField] Transform spawnPoint;
    
    protected override void Attack()
    {
        base.Attack();
        float dist = Vector2.Distance(Utils.MakeV2(transform.position), Utils.MakeV2(MyTarget.position));
        if (dist <= attackRange)
        {
            SpawnProjectile(spawnPoint);
        }
        else
        {
            myLoco.moveCurrent = E_Loco.EnMovement.Chase;
        }
    }

}

