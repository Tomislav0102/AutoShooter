using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class EffectTrigger : MonoBehaviour, IIniSpell
{
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value;
            if (value.colliderType == ColliderType.Sphere) _arcCalculator = GetComponent<ArcCalculator>();
            if (solidCollider == null) return;
        
            solidCollider.enabled = value.transporter.bounce > 0;
            solidCollider.radius = value.mySphereCollider.radius + 0.5f; //if proj speed is > 10 collision might not work and radius needs to be increased 

        }
    }
    SpellMain _spell;
    [SerializeField] bool detectObstacles = true;
    [SerializeField] bool onEnter = true;
    [SerializeField] bool onFakeStay;
    [SerializeField] bool onExit;
    ArcCalculator _arcCalculator;
    [SerializeField] SphereCollider solidCollider;

    public void Hit()
    {
        if (!onFakeStay) return;
        bool callEvent = true;
        foreach (Collider item in Spell.collidersDetected)
        {
            if (item == null) continue;
            Spell.HitGeneric(item, out _, callEvent);
            callEvent = false;
        }
    }


    public void OnTriggerEnterCallBack(Collider other)
    {
       // print($"1 {other.gameObject.name}");
        if (_arcCalculator != null && !_arcCalculator.TargetInsideArc(other.transform.position)) return;
      //  print($"2 {other.gameObject.name}");
        if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;
     //   print($"3 {other.gameObject.name}");
        if (Spell.collidersDetected.Contains(other)) return;
    //    print($"4 {other.gameObject.name}");
        Spell.collidersDetected.Add(other);
        if (!onEnter || onFakeStay) return;
     //  print($"5 {other.gameObject.name}");

        if (other.TryGetComponent(out Brain targetBrain))
        {
            Spell.HitGeneric(targetBrain, out _);

            switch (Spell.specialty)
            {
                case SpellMain.Specialty.Projectile:
                    if (Spell.OwnersBrain == targetBrain) //needed for reflection?
                    {
                        Spell.collidersDetected.Clear();
                        return;
                    }
                    if (Spell.transporter.CanRicochet(other) || Spell.transporter.CanPierce()) return;
                    break;
            }
            Spell.MyPhase = SpellMain.Phase.EndStart;
        }
        else if (detectObstacles) 
        {
            Spell.MyPhase = SpellMain.Phase.EndStart;
        }
       
    }


    public void OnTriggerExitCallBack(Collider other)
    {
       // print(1);
        if (_arcCalculator != null && !_arcCalculator.TargetInsideArc(other.transform.position)) return;
     //   print(2);
        if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;
      //  print(3);
        if (!Spell.collidersDetected.Contains(other)) return;
      // print(4);
        Spell.collidersDetected.Remove(other);
        
        if (!onExit) return;
        Spell.HitExit(other);
    }
    
    public void OnCollisionEnterCallBack(Collision collision)
    {
        if (Spell.debug)
        {
            print(Spell.gameObject.name + " collided with " + collision.gameObject.name);
            for (int i = 0; i < collision.contactCount; i++)
            {
                print($"normal is {collision.GetContact(i).normal}");
            }
        }
        if (!Spell.transporter.CanBounce(collision.GetContact(0).normal)) Spell.MyPhase = SpellMain.Phase.EndStart;
    }


}
