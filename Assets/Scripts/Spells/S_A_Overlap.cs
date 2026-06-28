using System;
using System.Collections.Generic;
using UnityEngine;

public class S_A_Overlap : S_A
{
    public override void InitializeMe(SpellMain mainSpell)
    {
        base.InitializeMe(mainSpell);
        main.mySphereCollider.enabled = false;
        main.myCapsuleCollider.enabled = false;
    }

    protected override void Hit()
    {
        Collider[] colliders = null;
        bool eventCall = false;
        int layerMask = Utils.MyLayers(new string[] { Ga.me.gameData.layActors, Ga.me.gameData.laySpell });

        switch (main.colliderType)
        {
            case ColliderType.Sphere:
                colliders = Physics.OverlapSphere(main.myTransform.position,
                    areaOfEffect * 0.5f, layerMask);
                
                //optimized version
                Collider[] collsNonAlloc = new Collider[10];
                int numNonAlloc = Physics.OverlapSphereNonAlloc(main.myTransform.position, areaOfEffect * 0.5f, collsNonAlloc, layerMask);
                for (int i = 0; i < numNonAlloc; i++)
                {
                   if (false) print(collsNonAlloc[i].name);
                }
                break;
            case ColliderType.Capsule:
                Vector3 startPos = main.myTransform.position + 0.5f * main.myTransform.forward;
                Vector3 endPos = startPos + Mathf.Max(0, areaOfEffect - 1) * main.myTransform.forward;
                colliders = Physics.OverlapCapsule(startPos, endPos,
                    0.5f, layerMask);
                break;
        }
        foreach (Collider item in colliders)
        {
            HitGeneric(item, out Brain targetBrain);
            if (targetBrain != null)
            {
                main.onHitTarget?.Invoke(targetBrain);
                eventCall = true;
            }
        }
        if (!eventCall) main.onHitTarget?.Invoke(null);
    }

}

