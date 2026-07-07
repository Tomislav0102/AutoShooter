using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Animations.Rigging;

public class PC_Knight : P_Combat
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
                   { Element.Electricity, 2f },
                 // { Element.Poison, Br.myChar.GetStat(Stats.MeleeDamage) },
                 // { Element.Fire, Br.myChar.GetStat(Stats.MeleeDamage) },
            };
            damRanged = new Dictionary<Element, float>()
            {
                { Element.Physical, Br.character.GetStat(Stats.RangedDamage) },
            };
            damUltimate = new Dictionary<Element, float>()
            {
                { Element.Electricity, Br.character.GetStat(Stats.MagicDamage) },
            };
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
            
            IsInitialized = true;
        }
    }

    [Title("Knight")]
    [SerializeField] Transform myShield;
    [SerializeField] int power = 2;
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
        injectHealth = new InjectHealth(damMelee, true, power);
        melee.InitializeMe(Br, injectHealth);
    }

    public override void FromAnimEv_Ultimate(int num = 0)
    {
        base.FromAnimEv_Ultimate(num);
        SpellMain dash = Instantiate(Ga.me.spells.dash, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        dash.transporter.target = Br.myTransform;
        injectHealth = new InjectHealth(damUltimate, false, power);
        dash.InitializeMe(Br, injectHealth);
        Br.myRigid.AddRelativeForce(power * 5 * Vector3.forward, ForceMode.VelocityChange);
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
                // float chance = 0.05f;
                // if (Random.value > chance || Br.combat.MyTarget == null) return;
                // Vector3 dir = Utils.Direction(myShield.position, Br.combat.MyTarget.position);
                // SpellMain shieldThrow = Instantiate(Ga.me.spells.shieldThrow, myShield.position, Quaternion.LookRotation(dir), Ga.me.spells.myTransform);
                // shieldThrow.InitializeMe(Br, new InjectHealth(damRanged));
                break;
            case CombatEvent.GetHit:
                break;
            case CombatEvent.Block:
               // StartCoroutine(SpellPushDelay());
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
