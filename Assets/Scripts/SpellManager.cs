   using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SpellManager : MonoBehaviour
{
   [HideInInspector] public Transform myTransform;
   public SpellControl meleePlayer;
   public SpellControl meleeEnemy;
   [Title("Hit")]
   // public Spell meleeEnemy;
   // public Spell meleePlayer;
   public Spell pushAll;
   public Spell lightningStrike;
   [Title("Bullet")]
   public Spell projectileEnemy;
   public Spell projectilePlayer;
   [Title("Area")]
   public Spell fireWalk;
   [Title("Hook")]
   public Spell hookHealDot;
   [Title("Lob")]
   public Spell lobCarrierFireball;
   [Title("Homing")]
   public Spell homing;
   [Title("Shield")]
   public Spell shieldPlayer;
   [Title("Swords")] 
   public Spell swordFire;
   public Spell swordIce;
   public Spell swordEle;
   
   void Awake() 
   {
      myTransform = transform;
   }
}
