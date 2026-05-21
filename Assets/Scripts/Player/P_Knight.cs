using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class P_Knight : PlayerCombat
{
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            damMelee = new Dictionary<Element, float>()
            {
                 //{ Element.Physical, Br.myChar.GetStat(Stats.MeleeDamage) },
                  { Element.Ice, Br.myChar.GetStat(Stats.RangedDamage) },
                  { Element.Magic, 2f },
                 // { Element.Poison, Br.myChar.GetStat(Stats.MeleeDamage) },
                 // { Element.Fire, Br.myChar.GetStat(Stats.MeleeDamage) },
            };
            damUltimate = new Dictionary<Element, float>()
            {
                { Element.Physical, -Br.myChar.GetStat(Stats.MagicDamage) },
            };
            IsInitialized = true;
            
            SpellMain reflect = Instantiate(Ga.me.spells.reflectProjectile, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            reflect.spell.followTarget = Br.myTransform;
            reflect.InitializeMe(Br);
        }
    }

    [Title("Knight")]
    [SerializeField][Range(1, 10)] int dashPower = 4;
    float _timerBlockReady;
    const int CONST_BlockTimer = 2;
    
    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        SpellMain melee = Instantiate(Ga.me.spells.meleePlayer, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
        melee.onHitTarget += (ITakeDamage it) =>
        {
            Br.myChar.RegisterSkillReqIncrease(it == null ? SkillReq.Miss : SkillReq.Hit);
        };
        melee.InitializeMe(Br, damMelee);
    }

    public override void FromAnimEv_Ultimate(int num = 0)
    {
        base.FromAnimEv_Ultimate(num);
        SpellMain arc = Instantiate(Ga.me.spells.sweepingArc, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
        arc.InitializeMe(Br, new Dictionary<Element, float>()
        {
            { Element.Magic, Br.myChar.GetStat(Stats.MagicDamage) },
        });

        // SpellControl heal = Instantiate(Ga.me.spells.heal, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        // heal.spell.anchor = Br.myTransform;
        // heal.InitializeMe(Br, damUltimate);
    }

    void Update()
    {
        if (_timerBlockReady >= 0f)
        {
            _timerBlockReady -= Time.deltaTime;
        }
    }

    protected override void CallEv_OnUltimateActivated()
    {
        base.CallEv_OnUltimateActivated();
        Br.loco.CastSpell();
    }

    public override void HealthHitCallback(InjectHealth injectHealth)
    {
        if (_timerBlockReady > 0f)
        {
            counterHitReceived = 0;
            return;
        }
        base.HealthHitCallback(injectHealth);
        
        float rdn = Random.value * counterHitReceived;
        if (rdn >= 1)
        {
            _timerBlockReady = CONST_BlockTimer;
            counterHitReceived = 0;
            Br.loco.Block();
            StartCoroutine(SpellPushDelay());
        }

        return;
        IEnumerator SpellPushDelay()
        {
            yield return new WaitForSeconds(0.1f);
            SpellMain push = Instantiate(Ga.me.spells.push, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            push.InitializeMe(Br);
        }
    }


}
