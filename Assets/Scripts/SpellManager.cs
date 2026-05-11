using Sirenix.OdinInspector;
using UnityEngine;


public class SpellManager : MonoBehaviour
{
   public Transform myTransform;
   public SpellControl meleePlayer;
   public SpellControl meleeEnemy;
   public SpellControl bulletPlayer;
   public SpellControl bulletEnemy;
   public SpellControl homingMissile;
   public SpellControl lobCarrySomething;
   public SpellControl lightningStrike;
   public SpellControl heal;
   public SpellControl shieldFromProjectiles;
   public SpellControl push;
   [Title("Groups")] 
   public SpellGroup groupWalkTrail;
   public SpellGroup groupOrbitalShields;
   public SpellGroup groupOrbitalSwordsFire;
   public SpellGroup groupOrbitalSwordsIce;
   public SpellGroup groupOrbitalSwordsElectric;
   [Title("Fireball set")] 
   public SpellControl carryFireball;
   public SpellControl explosionFire;
   public SpellControl areFire;
}
