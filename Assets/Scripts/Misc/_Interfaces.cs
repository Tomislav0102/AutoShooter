using System.Collections;
using UnityEngine;

public interface IInit
{
    void Initialize(Brain brain);
    bool IsReady { get; set; }
}

public interface ITakeDamage : IInit
{
    void TakeDamage(float damageTaken, Transform attacker = null);
};


public interface IObstacle { };
