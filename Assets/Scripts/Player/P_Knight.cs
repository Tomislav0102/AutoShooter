using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
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
                 // { Element.Physical, Br.myChar.GetStat(Stats.MeleeDamage) },
                  // { Element.Ice, Br.myChar.GetStat(Stats.RangedDamage) },
                  // { Element.Magic, 2f },
                 // { Element.Poison, Br.myChar.GetStat(Stats.MeleeDamage) },
                 // { Element.Fire, Br.myChar.GetStat(Stats.MeleeDamage) },
            };
            damRanged = new Dictionary<Element, float>()
            {
                { Element.Physical, Br.myChar.GetStat(Stats.RangedDamage) },
            };
            damUltimate = new Dictionary<Element, float>()
            {
                { Element.Physical, -Br.myChar.GetStat(Stats.MagicDamage) },
            };
            IsInitialized = true;

            switch (startActive)
            {
                case 0:
                    SpellMain reflect = Instantiate(Ga.me.spells.reflectProjectile, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    reflect.transporter.target = Br.myTransform;
                    reflect.InitializeMe(Br);
                    break;
                case 1:
                    SpellMain aura = Instantiate(Ga.me.spells.auraLowerAttSpeed, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform); 
                    aura.transporter.target = Br.myTransform;
                    aura.InitializeMe(Br);
                    break;
            }
            
            
        }
    }

    [Title("Knight")]
    [SerializeField] Transform myShield;
    [SerializeField][Range(1, 10)] int dashPower = 4;
    public int startActive;
    
    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        SpellMain melee = Instantiate(Ga.me.spells.meleePlayer, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
        melee.onHitTarget += (Brain br) =>
        {
            if (br == null) CombatEventRegistered(CombatEvent.Miss);
            else CombatEventRegistered(CombatEvent.Hit, br);
        };
        melee.InitializeMe(Br, damMelee);
    }

    public override void FromAnimEv_Ultimate(int num = 0)
    {
        base.FromAnimEv_Ultimate(num);

        SpellMain heal = Instantiate(Ga.me.spells.heal, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        heal.transporter.target = Br.myTransform;
        heal.InitializeMe(Br, damUltimate);
    }

    protected override void CallEv_OnUltimateActivated()
    {
        base.CallEv_OnUltimateActivated();
        Br.loco.CastSpell();
    }

    public override void CombatEventRegistered(CombatEvent combatEvent, Brain otherBrain = null)
    {
        base.CombatEventRegistered(combatEvent, otherBrain);
        string st = otherBrain == null ? "" : $"on {otherBrain.name}";
//       print($"{combatEvent} {st}");
        switch (combatEvent)
        {
            case CombatEvent.Strike:
                // if (!Br.health.IsAtFullHealth()) return;
                // SpellMain arc = Instantiate(Ga.me.spells.sweepingArc, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                // arc.InitializeMe(Br, new Dictionary<Element, float>()
                // {
                //     { Element.Magic, Br.myChar.GetStat(Stats.MagicDamage) },
                // });
                break;
            case CombatEvent.Hit:
                break;
            case CombatEvent.Miss:
                // if (Random.value > 0.05f) return;
                // SpellMain shieldThrow = Instantiate(Ga.me.spells.shieldThrow, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                // shieldThrow.InitializeMe(Br, damRanged);
                break;
            case CombatEvent.GetHit:
                break;
            case CombatEvent.Block:
              //  StartCoroutine(SpellPushDelay());
                break;
            case CombatEvent.Kill:
                // if (otherBrain.myChar.GetStat(Stats.MagicDamage) >= Br.myChar.GetStat(Stats.MagicDamage))
                // {
                //     Br.myChar.ChangeStat(Character.BuffType.Skill,Stats.MagicDamage, 1);
                // }
                break;
        }
        return;
        
        IEnumerator SpellPushDelay()
        {
            yield return new WaitForSeconds(0.1f);
            SpellMain push = Instantiate(Ga.me.spells.push, myShield.position, Quaternion.identity, Ga.me.spells.myTransform);
            push.InitializeMe(Br);
        }
    }



}
