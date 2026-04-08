using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class P_Knight : PlayerCombat
{
    [Title("Knight")]
    [SerializeField][Range(1, 10)] int dashPower = 4;
    float _timerBlockReady;
    const int CONST_BlockTimer = 2;
    
    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        Spell melee = Instantiate(Ga.me.spells.meleePlayer,Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        melee.myTransform.position += melee.radius * Br.myTransform.forward;
        melee.InitializeMe(Br);
    }

    public override void FromAnimEv_SpellCast(int num = 0)
    {
        base.FromAnimEv_SpellCast(num);
       Spell sp = Instantiate(Ga.me.spells.hookHealDot, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
       sp.anchor = Br.myTransform;
       sp.InitializeMe(Br);
        
    }

    protected override void Update()
    {
        base.Update();
        if (_timerBlockReady >= 0f)
        {
            _timerBlockReady -= Time.deltaTime;
        }
    }

    protected override void CallEv_OnSpecialActivated()
    {
        base.CallEv_OnSpecialActivated();
        Br.loco.CastSpell();
    }

    public override void HealthHitCallback(DamageData damageData)
    {
        if (_timerBlockReady > 0f)
        {
            counterHit = 0;
            return;
        }
        base.HealthHitCallback(damageData);
        
        float rdn = Random.value * counterHit;
        if (rdn >= 1)
        {
            _timerBlockReady = CONST_BlockTimer;
            counterHit = 0;
            Br.loco.Block();
            StartCoroutine(SpellPushDelay());
        }

        IEnumerator SpellPushDelay()
        {
            yield return new WaitForSeconds(0.1f);
            Spell push = Instantiate(Ga.me.spells.pushAll, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            push.InitializeMe(Br);
        }
    }


}
