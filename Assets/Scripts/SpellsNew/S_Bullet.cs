using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class S_Bullet : Spell
{
    [SerializeField] SphereCollider solidCollider;
    BulletTransporter _myBulletTransporter;
    
    
    public override void InitializeMe(SpellControl mainSpell)
    {
        base.InitializeMe(mainSpell);
        _myBulletTransporter = main.transporter as  BulletTransporter;
        solidCollider.enabled = _myBulletTransporter.bounce > 0;
        solidCollider.radius = main.mySphereCollider.radius + 0.01f;
        spellParticles.InitializeMe(areaOfEffect);
    }

    protected override void CallEv_OnTriggerEnter(Collider other)
    {
        base.CallEv_OnTriggerEnter(other);
        if (injectHealthData.damage.Count > 0 && 
            other.TryGetComponent(out ITakeDamage takeDamage) && 
            Utils.CanTargetFaction(myFaction, takeDamage.Br.faction, myFactionTarget))
        {
            takeDamage.TakeDamage(injectHealthData);
            if (_myBulletTransporter.ricochet > 0)
            {
                float range = 3f;
                Collider[] colliders = Physics.OverlapSphere(main.myTransform.position, range,
                    Utils.MyLayer(Ga.me.gameData.layActors));
                List<Transform> myTargets = new List<Transform>();
                foreach (Collider item in colliders)
                {
                    if (item == other) continue;
                    if (item.TryGetComponent(out ITakeDamage itemTakeDamage) &&
                        Utils.CanTargetFaction(myFaction, itemTakeDamage.Br.faction, myFactionTarget))
                    {
                        myTargets.Add(item.transform);
                    }
                }
        
                if (myTargets.Count > 0)
                {
                    Vector3 dir = myTargets[Random.Range(0, myTargets.Count)].position - main.myTransform.position;
                    _myBulletTransporter.RicochetMethod(dir);
                }
                else SetPierce();
            }
            else SetPierce();
        
        
            void SetPierce()
            {
                if (_myBulletTransporter.pierce > 0) _myBulletTransporter.pierce--;
                else OnEnd();
            }
        
        }
    }

    protected override void CallEv_OnCollisionEnter(Collision collision)
    {
        base.CallEv_OnCollisionEnter(collision);
        if (_myBulletTransporter.bounce > 0)
        {
            _myBulletTransporter.BounceMethod(collision.GetContact(0).normal);
        }
        else OnEnd();
        
    }


   //  void OnTriggerEnter(Collider other)
   //  {
   //      if (injectHealthData.damage.Count > 0 && 
   //          other.TryGetComponent(out ITakeDamage takeDamage) && 
   //          Utils.CanTargetFaction(myFaction, takeDamage.Br.faction, myFactionTarget))
   //      {
   //          takeDamage.TakeDamage(injectHealthData);
   //          if (ricochet > 0)
   //          {
   //              float range = 3f;
   //              Collider[] colliders = Physics.OverlapSphere(comp.myTransform.position, range);
   //              List<Transform> myTargets = new List<Transform>();
   //              foreach (Collider item in colliders)
   //              {
   //                  if (item != other && myFaction != takeDamage.Br.faction) myTargets.Add(item.transform);
   //              }
   //  
   //              if (myTargets.Count > 0)
   //              {
   //                  Vector3 dir = myTargets[Random.Range(0, myTargets.Count)].position - comp.myTransform.position;
   //                  comp. myTransform.forward = dir.normalized;
   //                  ricochet--;
   //                  SetSpeed();
   //              }
   //              else SetPierce();
   //          }
   //          else SetPierce();
   //  
   //  
   //          void SetPierce()
   //          {
   //              if (pierce > 0) pierce--;
   //              else OnEnd();
   //          }
   //  
   //      }
   //  }
}
// public class S_Bullet : Spell
// {
//     public int ricochet;
//     public int pierce;
//     public int bounce;
//    [SerializeField] SphereCollider solid;
//
//     public override void InitializeMe(Brain brain, Dictionary<Element, float> damage)
//     {
//         base.InitializeMe(brain, damage);
//         comp.myRigid.isKinematic = false;
//         SetSpeed();
//         solid.enabled = bounce > 0;
//         solid.radius = comp.mySphereCollider.radius + 0.01f;
//         Physics.IgnoreCollision(solid, brain.myCollider);
//     }
//     
//     
//     void OnTriggerEnter(Collider other)
//     {
//         if (injectHealthData.damage.Count > 0 && 
//             other.TryGetComponent(out ITakeDamage takeDamage) && 
//             Utils.CanTargetFaction(myFaction, takeDamage.Br.faction, myFactionTarget))
//         {
//             takeDamage.TakeDamage(injectHealthData);
//             if (ricochet > 0)
//             {
//                 float range = 3f;
//                 Collider[] colliders = Physics.OverlapSphere(comp.myTransform.position, range);
//                 List<Transform> myTargets = new List<Transform>();
//                 foreach (Collider item in colliders)
//                 {
//                     if (item != other && myFaction != takeDamage.Br.faction) myTargets.Add(item.transform);
//                 }
//     
//                 if (myTargets.Count > 0)
//                 {
//                     Vector3 dir = myTargets[Random.Range(0, myTargets.Count)].position - comp.myTransform.position;
//                     comp. myTransform.forward = dir.normalized;
//                     ricochet--;
//                     SetSpeed();
//                 }
//                 else SetPierce();
//             }
//             else SetPierce();
//     
//     
//             void SetPierce()
//             {
//                 if (pierce > 0) pierce--;
//                 else OnEnd();
//             }
//     
//         }
//     }
//     
//     void OnCollisionEnter(Collision other)
//     {
//         if (other.collider.TryGetComponent(out IObstacle obstacle))
//         {
//             if (bounce > 0)
//             {
//                 bounce--;
//                 Vector3 dir = Vector3.Reflect(comp.myTransform.forward, other.GetContact(0).normal);
//                 comp.myTransform.forward = dir.normalized;
//                 SetSpeed();
//             }
//             else OnEnd();
//         }
//     }
//
// }
