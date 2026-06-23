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
   public SpellMain shieldFromProjectiles;
   public SpellMain swordFire, swordIce, swordElectricity;
   [Title("Mage Fire")]
   public SpellMain flameThrower;
   public SpellMain meteorStrike;
   public SpellMain walkTrailSingle;
   public SpellMain armageddon;
   [Title("Mage Lightning")]
   public SpellMain lightningStrike;
   [Title("Knight")]
   public SpellMain meleePlayer;
   public SpellMain push;
   public SpellMain pushPulsating;
   public SpellMain sweepingArc;
   public SpellMain reflectProjectile;
   public SpellMain shieldThrow;
   public SpellMain auraLowerAttSpeed;
   public SpellMain dash;
   [Title("Groups")] 
   public SpellGroup groupWalkTrail;
   public SpellGroup groupOrbitalShields;
   public SpellGroup groupOrbitalSwords;
   [Title("Fireball set")] 
   public SpellMain carryFireball;
   public SpellMain explosionFire;
   public SpellMain areFire;
   
}
