using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class S_Bullet : Spell
{
    [Title("Bullet")]
    public int ricochet;
    public int pierce;
    public int bounce;
    [SerializeField] SphereCollider solid;

    public override void InitializeMe(Brain brain)
    {
        base.InitializeMe(brain);
        comp.mySphereCollider.enabled = true;
        comp.myRigid.isKinematic = false;
        SetSpeed();
        solid.enabled = bounce > 0;
        solid.radius = comp.mySphereCollider.radius + 0.01f;
        Physics.IgnoreCollision(solid, brain.myCollider);
    }

    void OnTriggerEnter(Collider other)
    {
        if (damData.damage > 0 && other.TryGetComponent(out ITakeDamage takeDamage) && factionsToTarget.Contains(takeDamage.Br.faction))
        {
            takeDamage.TakeDamage(damData);
            if (ricochet > 0)
            {
                float range = 3f;
                Collider[] colliders = Physics.OverlapSphere(comp.myTransform.position, range);
                List<Transform> myTargets = new List<Transform>();
                foreach (Collider item in colliders)
                {
                    if (item != other && myFaction != takeDamage.Br.faction) myTargets.Add(item.transform);
                }

                if (myTargets.Count > 0)
                {
                    Vector3 dir = myTargets[Random.Range(0, myTargets.Count)].position - comp.myTransform.position;
                    comp. myTransform.forward = dir.normalized;
                    ricochet--;
                    SetSpeed();
                }
                else SetPierce();
            }
            else SetPierce();


            void SetPierce()
            {
                if (pierce > 0) pierce--;
                else OnEnd();
            }

        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out IObstacle obstacle))
        {
            if (bounce > 0)
            {
                bounce--;
                Vector3 dir = Vector3.Reflect(comp.myTransform.forward, other.GetContact(0).normal);
                comp.myTransform.forward = dir.normalized;
                SetSpeed();
            }
            else OnEnd();
        }
    }

}
