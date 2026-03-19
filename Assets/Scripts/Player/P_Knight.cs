using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class P_Knight : PlayerCombat
{
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
    
    protected override void CallEv_OnSpecialActivated()
    {
        base.CallEv_OnSpecialActivated();
        Br.loco.Dash(Br.myTransform.forward, dashPower);
    }
    

    public override void HealthHitCallback(DamageData damageData)
    {
        base.HealthHitCallback(damageData);
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
    
    void OnTriggerEnter(Collider other)
    {
        print((other.name + " has entered the knight"));
        // if (other.TryGetComponent(out Brain brain))
        // {
        //     if (brain.loco != null) brain.loco.KnockBack(Utils.Direction(Br.myTransform.position, other.transform
        //         .position), 5);
        // }
    }

}
