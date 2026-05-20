using Sirenix.OdinInspector;
using UnityEngine;


public class SpellManager : MonoBehaviour
{
   public Transform myTransform;
   public SpellMain meleePlayer;
   public SpellMain meleeEnemy;
   public SpellMain bulletPlayer;
   public SpellMain bulletEnemy;
   public SpellMain homingMissile;
   public SpellMain lobCarrySomething;
   public SpellMain lightningStrike;
   public SpellMain heal;
   public SpellMain shieldFromProjectiles;
   public SpellMain push;
   public SpellMain sweepingArc;
   public SpellMain armageddon;
   [Title("Groups")] 
   public SpellGroup groupWalkTrail;
   public SpellGroup groupOrbitalShields;
   public SpellGroup groupOrbitalSwordsFire;
   public SpellGroup groupOrbitalSwordsIce;
   public SpellGroup groupOrbitalSwordsElectric;
   [Title("Fireball set")] 
   public SpellMain carryFireball;
   public SpellMain explosionFire;
   public SpellMain areFire;
   
}
