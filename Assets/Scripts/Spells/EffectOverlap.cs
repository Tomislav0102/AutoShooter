using System;
using System.Collections.Generic;
using UnityEngine;


public class EffectOverlap : MonoBehaviour, IIniSpell
{
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell= value;
            value.mySphereCollider.enabled = false;
            value.myCapsuleCollider.enabled = false;

        }
    }
    SpellMain _spell;

    public void Hit()
    {
        Collider[] colliders = System.Array.Empty<Collider>();
        bool eventCall = false;
        // int layerMask = Utils.MyLayers(new string[] { Ga.me.gameData.layActors, Ga.me.gameData.laySpell });
        int layerMask = Utils.MyLayer(Spell.isInterrupt ? Ga.me.gameData.laySpell : Ga.me.gameData.layActors);
        switch (Spell.colliderType)
        {
            case ColliderType.Sphere:
                colliders = Physics.OverlapSphere(Spell.myTransform.position,
                    Spell.areaOfEffect * 0.5f, layerMask);

                //optimized version
                Collider[] collsNonAlloc = new Collider[10];
                int numNonAlloc = Physics.OverlapSphereNonAlloc(Spell.myTransform.position, Spell.areaOfEffect * 0.5f, collsNonAlloc, layerMask);
                for (int i = 0; i < numNonAlloc; i++)
                {
                    if (false) print(collsNonAlloc[i].name);
                }
                break;
            case ColliderType.Capsule:
                Vector3 startPos = Spell.myTransform.position + 0.5f * Spell.myTransform.forward;
                Vector3 endPos = startPos + Mathf.Max(0, Spell.areaOfEffect - 1) * Spell.myTransform.forward;
                colliders = Physics.OverlapCapsule(startPos, endPos,
                    0.5f, layerMask);
                break;
        }
        foreach (Collider item in colliders)
        {
            Spell.HitGeneric(item, out Brain targetBrain);
            if (targetBrain != null)
            {
                Spell.onHitTarget?.Invoke(targetBrain);
                eventCall = true;
            }
        }
        if (!eventCall) Spell.onHitTarget?.Invoke(null);
    }

}

