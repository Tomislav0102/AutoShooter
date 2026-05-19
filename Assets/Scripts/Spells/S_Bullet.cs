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
        _myBulletTransporter = main.transporter as BulletTransporter;
        solidCollider.enabled = _myBulletTransporter.bounce > 0;
        solidCollider.radius = main.mySphereCollider.radius + 0.01f;
        spellParticles.InitializeMe(mainSpell);
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
                else main.spell.MyPhase = Phase.EndStart;
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
        else main.spell.MyPhase = Phase.EndStart;

    }
}
