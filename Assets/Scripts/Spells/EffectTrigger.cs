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
        }
    }
    SpellMain _spell;
    [SerializeField] bool onEnter = true;
    [SerializeField] bool onFakeStay;
    [SerializeField] bool onExit;
    [ShowIf(nameof(onExit))] public SpellMain effectAtExit; //public so it can be added through code

    enum ColliderPart
    {
        Whole,
        FrontHalf,
        BackHalf,
    }
    [SerializeField] ColliderPart colliderPart;

    
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
        if (!CheckColliderType(other.transform.position)) return;
        if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;

        if (Spell.collidersDetected.Contains(other)) return;
        Spell.collidersDetected.Add(other);
        if (!onEnter || onFakeStay) return;
        
        Spell.HitGeneric(other, out Brain targetBrain);
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
        if (!CheckColliderType(other.transform.position)) return;
        if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;
        if (!Spell.collidersDetected.Contains(other)) return;
        Spell.collidersDetected.Remove(other);
        
        if (!onExit || effectAtExit == null) return;
    }

    bool CheckColliderType(Vector3 pos)
    {
        float posZ =  Spell.myTransform.InverseTransformPoint(pos).z;
        switch (colliderPart)
        {
            case ColliderPart.FrontHalf:
                if (posZ < 0) return false;
                break;
            case ColliderPart.BackHalf:
                if (posZ > 0) return  false;
                break;
        }
        return true;
    }
}
