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


    public override void InitializeMe(SpellMain mainSpell)
    {
        base.InitializeMe(mainSpell);
        _myBulletTransporter = main.transporter as BulletTransporter;
        if (solidCollider != null)
        {
            solidCollider.enabled = _myBulletTransporter.bounce > 0;
            solidCollider.radius = main.mySphereCollider.radius + 0.01f;
        }
    }

    public override void OnTriggerEnterCallBack(Collider other)
    {
        base.OnTriggerEnterCallBack(other);
        HitGeneric(other, out Brain targetBrain);
        if (targetBrain != null)
        {
            main.onHitTarget?.Invoke(targetBrain);
            if (_myBulletTransporter.ricochet > 0)
            {
                float range = 3f;
                Collider[] colliders = Physics.OverlapSphere(main.myTransform.position, range,
                    Utils.MyLayer(Ga.me.gameData.layActors));
                List<Transform> myTargets = new List<Transform>();
                foreach (Collider item in colliders)
                {
                    if (item == other) continue;
                    if (item.TryGetComponent(out Brain ricochetTargetBrain) &&
                        Utils.CanTargetFaction(main.OwnersBrain.Faction, ricochetTargetBrain.Faction, myFactionTarget))
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
                else main.spell.MyPhase = Phase.EndStart;
            }

        }

        if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;
        main.spell.MyPhase = Phase.EndStart;
    }

    public override void OnCollisionEnterCallBack(Collision collision)
    {
        base.OnCollisionEnterCallBack(collision);
        if (_myBulletTransporter.bounce > 0)
        {
            _myBulletTransporter.BounceMethod(collision.GetContact(0).normal);
        }
        else main.spell.MyPhase = Phase.EndStart;

    }
}


// public override void OnTriggerEnterCallBack(Collider other)
// {
//     base.OnTriggerEnterCallBack(other);
//     if (injectHealth.damage.Count > 0 &&
//         other.TryGetComponent(out Brain targetBrain) &&
//         Utils.CanTargetFaction(main.OwnersBrain.Faction, targetBrain.Faction, myFactionTarget))
//     {
//         targetBrain.health.TakeDamage(injectHealth);
//         main.onHitTarget?.Invoke(targetBrain);
//         if (_myBulletTransporter.ricochet > 0)
//         {
//             float range = 3f;
//             Collider[] colliders = Physics.OverlapSphere(main.myTransform.position, range,
//                 Utils.MyLayer(Ga.me.gameData.layActors));
//             List<Transform> myTargets = new List<Transform>();
//             foreach (Collider item in colliders)
//             {
//                 if (item == other) continue;
//                 if (item.TryGetComponent(out Brain ricochetTargetBrain) &&
//                     Utils.CanTargetFaction(main.OwnersBrain.Faction, ricochetTargetBrain.Faction, myFactionTarget))
//                 {
//                     myTargets.Add(item.transform);
//                 }
//             }
//
//             if (myTargets.Count > 0)
//             {
//                 Vector3 dir = myTargets[Random.Range(0, myTargets.Count)].position - main.myTransform.position;
//                 _myBulletTransporter.RicochetMethod(dir);
//             }
//             else SetPierce();
//         }
//         else SetPierce();
//
//
//         void SetPierce()
//         {
//             if (_myBulletTransporter.pierce > 0) _myBulletTransporter.pierce--;
//             else main.spell.MyPhase = Phase.EndStart;
//         }
//
//     }
//     if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;
//     main.spell.MyPhase = Phase.EndStart;
//}
