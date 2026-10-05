using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EffectProjectile : MonoBehaviour, IIniSpell
{
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value;
           // _myBulletTransporter = value.transporter as BulletTransporter;
            if (solidCollider == null) return;
        
            //solidCollider.enabled = _myBulletTransporter.bounce > 0;
            solidCollider.radius = value.mySphereCollider.radius + 0.01f;
        }
    }
    SpellMain _spell;
    [SerializeField] SphereCollider solidCollider;
  //  BulletTransporter _myBulletTransporter;


    // public void OnTriggerEnterCallBack(Collider other)
    // {
    //     Spell.HitGeneric(other, out Brain targetBrain);
    //     if (Spell.OwnersBrain == targetBrain) return; //reflected spell
    //     
    //     if (targetBrain != null)
    //     {
    //         Spell.onHitTarget?.Invoke(targetBrain);
    //         if (_myBulletTransporter.ricochet > 0)
    //         {
    //             float range = 3f;
    //             Collider[] colliders = Physics.OverlapSphere(Spell.myTransform.position, range,
    //                 Utils.MyLayer(Ga.me.gameData.layActors));
    //             List<Transform> myTargets = new List<Transform>();
    //             foreach (Collider item in colliders)
    //             {
    //                 if (item == other) continue;
    //                 if (item.TryGetComponent(out Brain ricochetTargetBrain) &&
    //                     Utils.CanTargetFaction(Spell.OwnersBrain.Faction, ricochetTargetBrain.Faction, Spell.myFactionTarget))
    //                 {
    //                     myTargets.Add(item.transform);
    //                 }
    //             }
    //
    //             if (myTargets.Count > 0)
    //             {
    //                 Vector3 dir = myTargets[Random.Range(0, myTargets.Count)].position - Spell.myTransform.position;
    //                 _myBulletTransporter.RicochetMethod(dir);
    //             }
    //             else finishSpell();
    //         }
    //         else finishSpell();
    //         return;
    //
    //         void finishSpell()
    //         {
    //             if (_myBulletTransporter.pierce > 0)
    //             {
    //                 _myBulletTransporter.pierce--;
    //                 return;
    //             }
    //             Spell.MyPhase = SpellMain.Phase.EndStart;
    //         }
    //     
    //     }
    //
    //     if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;
    //     Spell.MyPhase = SpellMain.Phase.EndStart;
    // }
    //
    // public void OnCollisionEnterCallBack(Collision collision)
    // {
    //     if (_myBulletTransporter.bounce > 0)
    //     {
    //         _myBulletTransporter.BounceMethod(collision.GetContact(0).normal);
    //     }
    //     else Spell.MyPhase = SpellMain.Phase.EndStart;
    // }
}


