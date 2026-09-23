using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class HomingTransporter : SpellTransporter
{
    public override SpellMain Spell
    {
        get => base.Spell;
        set
        {
            base.Spell = value;
            value.myRigid.isKinematic = false;
        }
    }

    [SerializeField] float speed;
    

    void Update()
    {
        if (!Spell.spellActive ) return;
        if (target == null) return;
        Vector3 dir = Utils.Direction(Spell.myTransform.position, target.position);
        Spell.myTransform.rotation = Quaternion.Slerp(Spell.myTransform.rotation, Quaternion.LookRotation(dir), speed * Time.deltaTime);
    }
    
    
    void FixedUpdate()
    {
        SetSpeed(speed);
    }

}
