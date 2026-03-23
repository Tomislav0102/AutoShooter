using System;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
   [HideInInspector] public Transform myTransform;
   public GameObject melee;
   public GameObject meleeHard;
   public GameObject pushAll;
   public GameObject projectile;
   public GameObject fireball;
   public GameObject auraDamage;
   public GameObject hookHeal;
   public GameObject hookHealDot;
   public GameObject lobCarrierFireball;
   void Awake() 
   {
      myTransform = transform;
   }
}
