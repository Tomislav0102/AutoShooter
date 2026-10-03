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
        }
    }
    SpellMain _spell;
    [SerializeField] bool onEnter = true;
    [SerializeField] bool onFakeStay;
    [SerializeField] bool onExit;
    ArcCalculator _arcCalculator;
    
    public void Hit()
    {
        if (!onFakeStay) return;
        foreach (Collider item in Spell.collidersDetected)
        {
            if (item == null) continue;
            Spell.HitGeneric(item, out Brain targetBrain);
            Spell.onHitTarget?.Invoke(targetBrain);
        }
    }

    
    public void OnTriggerEnterCallBack(Collider other)
    {
        if (_arcCalculator != null && !_arcCalculator.TargetInsideArc(other.transform.position)) return;
        if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;
        if (Spell.collidersDetected.Contains(other)) return;
        Spell.collidersDetected.Add(other);
        if (!onEnter || onFakeStay) return;
        
        Spell.HitGeneric(other, out Brain targetBrain);
        if (Spell.OwnersBrain == targetBrain) return; //reflected spell

        Spell.onHitTarget?.Invoke(targetBrain);
        if (!Spell.isInterrupt)
        {
            Spell.MyPhase = SpellMain.Phase.EndStart;
            return;
        }

        if (targetBrain == Spell.OwnersBrain) //interrupt hits its owners spell so contact/trigger should be ignored
        {
            Spell.MyPhase = SpellMain.Phase.EndStart;
        }
       
    }

    
    public void OnTriggerExitCallBack(Collider other)
    {
        if (_arcCalculator != null && !_arcCalculator.TargetInsideArc(other.transform.position)) return;
        if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;
        if (!Spell.collidersDetected.Contains(other)) return;
        Spell.collidersDetected.Remove(other);
        
        if (!onExit) return;
        Spell.HitExit(other);
    }

}
