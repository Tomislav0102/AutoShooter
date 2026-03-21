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
   void Awake()
   {
      myTransform = transform;
   }
}
