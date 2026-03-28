using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SpellManager : MonoBehaviour
{
   [HideInInspector] public Transform myTransform;
   public GameObject melee;
   public GameObject meleeHard;
   public GameObject pushAll;
   public GameObject projectile;
   public GameObject fireball;
   public GameObject fireWalk;
   public GameObject hookHeal;
   public GameObject hookHealDot;
   public GameObject lobCarrierFireball;
   public GameObject homing;
   void Awake() 
   {
      myTransform = transform;
   }
}
