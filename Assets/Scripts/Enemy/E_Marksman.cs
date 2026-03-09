using UnityEngine;

public class E_Marksman : EnemyCombat
{
    [SerializeField] Transform spawnPoint;
    
    public override void AE_Attack(int num = 0)
    {
        base.AE_Attack(num);
        float dist = Vector2.Distance(Utils.MakeV2(transform.position), Utils.MakeV2(MyTarget.position));
        if (dist <= rangeRanged)
        {
            SpawnProjectile(spawnPoint);
        }
        else
        {
            enLoco.moveCurrent = E_Loco.EnMovement.Chase;
        }
    }

}

