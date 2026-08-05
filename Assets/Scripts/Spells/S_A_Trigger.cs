using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class S_A_Trigger : S_A
{
    [Title("Triggers")]
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
        
    
    protected override void Hit()
    {
        base.Hit();
        if (!onFakeStay) return;
        foreach (Collider item in collidersDetected)
        {
            if (item == null) continue;
            HitGeneric(item, out Brain targetBrain);
            main.onHitTarget?.Invoke(targetBrain);
        }
    }

    
    public override void OnTriggerEnterCallBack(Collider other)
    {
        base.OnTriggerEnterCallBack(other);
        if (!CheckColliderType(other.transform.position)) return;
        if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;

        if (collidersDetected.Contains(other)) return;
        collidersDetected.Add(other);
        if (!onEnter || onFakeStay) return;
        
        HitGeneric(other, out Brain targetBrain);
        main.onHitTarget?.Invoke(targetBrain);
        if (!main.isInterrupt)
        {
            main.spell.MyPhase = Phase.EndStart;
            return;
        }

        if (targetBrain == main.OwnersBrain) //interrupt hits its owners spell so contact/trigger should be ignored
        {
            main.spell.MyPhase = Phase.EndStart;
        }
       
    }

    
    public override void OnTriggerExitCallBack(Collider other)
    {
        base.OnTriggerExitCallBack(other);
        if (!CheckColliderType(other.transform.position)) return;
        if (other.gameObject.layer == LayerMask.NameToLayer(Ga.me.gameData.laySpellInterrupt)) return;
        if (!collidersDetected.Contains(other)) return;
        collidersDetected.Remove(other);
        
        if (!onExit || effectAtExit == null) return;
    }

    bool CheckColliderType(Vector3 pos)
    {
        float posZ =  main.myTransform.InverseTransformPoint(pos).z;
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
