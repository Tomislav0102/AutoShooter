using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class S_Ballistic : Spell
{
    [Title("Projectile")]
    public int ricochet;
    public int pierce;
    public int bounce;
    [SerializeField] Collider solidCollider;

    public override void InitializeMe(Faction fac)
    {
        base.InitializeMe(fac);
        myCollider.enabled = true;
        myRigid.isKinematic = false;
        SetSpeed();
        solidCollider.enabled = bounce > 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ITakeDamage takeDamage))
        {
            if (takeDamage.Br.faction != myData.faction)
            {
                takeDamage.TakeDamage(myData.damage);
                if (ricochet > 0)
                {
                    float range = 3f;
                    Collider[] colliders = Physics.OverlapSphere(myTransform.position, range);
                    List<Transform> myTargets = new List<Transform>();
                    foreach (Collider item in colliders)
                    {
                        if (item != other && myData.faction != takeDamage.Br.faction) myTargets.Add(item.transform);
                    }
                
                    if (myTargets.Count > 0)
                    {
                        Vector3 dir = myTargets[Random.Range(0, myTargets.Count)].position - myTransform.position;
                        myTransform.forward = dir.normalized;
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
    }
    
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out IObstacle obstacle))
        {
            if (bounce > 0)
            {
                bounce--;
                Vector3 dir = Vector3.Reflect(myTransform.forward, other.GetContact(0).normal);
                myTransform.forward = dir.normalized;
                SetSpeed();
            }
            else OnEnd();
        }
    }

}
