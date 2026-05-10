using Sirenix.OdinInspector;
using UnityEngine;


public class SpellManager : MonoBehaviour
{
   [HideInInspector] public Transform myTransform;
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
   public SpellGroup groupOrbitalShields;
   public SpellGroup groupWalkTrail;
   [Title("Fireball set")] 
   public SpellControl carryFireball;
   public SpellControl explosionFire;
   public SpellControl areFire;
   
   void Awake() 
   {
      myTransform = transform;
   }
}
