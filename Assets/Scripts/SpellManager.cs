   using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SpellManager : MonoBehaviour
{
   [HideInInspector] public Transform myTransform;
   public SpellControl meleePlayer;
   public SpellControl meleeEnemy;
   public SpellControl bulletPlayer;
   public SpellControl bulletEnemy;
   public SpellControl homingMissile;
   public SpellControl lobCarrySomething;
   [Title("Fireball set")] 
   public SpellControl carryFireball;
   public SpellControl explosionFire;
   public SpellControl areFire;
   [Title("Old")]
   public Spell pushAll;
   
   void Awake() 
   {
      myTransform = transform;
   }
}
