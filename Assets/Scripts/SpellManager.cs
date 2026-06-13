using Sirenix.OdinInspector;
using UnityEngine;


public class SpellManager : MonoBehaviour
{
   public Transform myTransform;
   public SpellMain meleeEnemy;
   public SpellMain bulletEnemy;
   public SpellMain lobCarrySomething;
   public SpellMain heal;
   [Title("Archer")]
   public SpellMain bulletPlayer;
   [Title("Mage")]
   public SpellMain homingMissile;
   public SpellMain flameThrower;
   public SpellMain lightningStrike;
   public SpellMain shieldFromProjectiles;
   public SpellMain armageddon;
   [Title("Knight")]
   public SpellMain meleePlayer;
   public SpellMain push;
   public SpellMain pushPulsating;
   public SpellMain sweepingArc;
   public SpellMain reflectProjectile;
   public SpellMain shieldThrow;
   public SpellMain auraLowerAttSpeed;
   [Title("Groups")] 
   public SpellGroup groupWalkTrail;
   public SpellGroup groupHomingMissile;
   public SpellGroup groupOrbitalShields;
   public SpellGroup groupOrbitalSwords;
   public SpellGroup groupFlamethrowers;
   [Title("Fireball set")] 
   public SpellMain carryFireball;
   public SpellMain explosionFire;
   public SpellMain areFire;
   
}
