using UnityEngine;

public interface ICharacter
{
    Transform MyTransform { get; set; }
    Transform MyTarget { get; set; }
}
public interface ITakeDamage
{
    void TakeDamage(float damageTaken, Transform attacker = null);
};
public interface IObstacle { };