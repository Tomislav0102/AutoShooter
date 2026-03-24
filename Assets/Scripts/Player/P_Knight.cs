using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class P_Knight : PlayerCombat
{
    [Title("Knight")]
    [SerializeField][Range(1, 10)] int dashPower = 4;
    
    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        Spell melee = Instantiate(Ga.me.spells.meleeHard,
            Br.myTransform.position,
            Quaternion.identity, Ga.me.spells.myTransform).GetComponent<Spell>();
        melee.myTransform.position += melee.radius * Br.myTransform.forward;
        melee.InitializeMe(Br);
    }

    public override void FromAnimEv_SpellCast(int num = 0)
    {
        base.FromAnimEv_SpellCast(num);
       Spell sp = Instantiate(Ga.me.spells.hookHealDot, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform).GetComponent<Spell>();
       sp.anchor = Br.myTransform;
       sp.InitializeMe(Br);
        
    }

    protected override void CallEv_OnSpecialActivated()
    {
        base.CallEv_OnSpecialActivated();
       Br.loco.CastSpell();
    }
    
    public override void HealthHitCallback(DamageData damageData)
    {
        base.HealthHitCallback(damageData);
        if (!damageData.canBeBlocked) return;
        float rdn = Random.value * counterHit;
        if (rdn >= 1)
        {
            counterHit = 0;
            Br.loco.Block();
            Spell push = Instantiate(Ga.me.spells.pushAll, Br.myTransform.position, Quaternion.identity, Ga.me.spells
                .myTransform).GetComponent<Spell>();
            push.InitializeMe(Br);
        }
    }
    

}
