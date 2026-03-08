using System.Collections;
using UnityEngine;

public interface IInit
{
    void Initialize(Brain brain);
    bool IsReady { get; set; }
}

public interface ILocomotion : IInit
{
    IEnumerator Dash();
};
public interface ICombat : IInit
{
    bool IsAttacking { get; set; }
    Transform MyTarget { get; set; }
    void AE_Attack(int num = 0);
}
public interface ITakeDamage : IInit
{
    void TakeDamage(float damageTaken, Transform attacker = null);
};


public interface IObstacle { };
