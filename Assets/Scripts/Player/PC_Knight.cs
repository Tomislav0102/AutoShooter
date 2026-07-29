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
    [SerializeField] int powerKnockback = 2;
    [SerializeField] int powerDash = 2;
    public int startActive;

    // public override void FromAnimEv_Attack(int num = 0)
    // {
    //     base.FromAnimEv_Attack(num);
    //     PassDataContainer container = new PassDataContainer()
    //     {
    //         canBeBlocked = true,
    //         data = new PassData[2]
    //         {
    //             new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { Br.character.GetStat(Stats.MeleeDamage) }),
    //             new PassDataKnockBack(5)
    //         }
    //     };
    //
    //     SpellMain melee = Instantiate(GetSpellByAttackType(AnimAttackType.Melee), Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
    //     melee.onHitTarget += (Brain br) =>
    //     {
    //         if (br == null) CombatEventRegistered(CombatEvent.Miss);
    //         else CombatEventRegistered(CombatEvent.Hit, br);
    //     };
    //     melee.InitializeMe(Br, container);
    // }
    //
    // public override void FromAnimEv_Ultimate(int num = 0)
    // {
    //     base.FromAnimEv_Ultimate(num);
    //     
    //     Br.loco.PushMe(Br.myTransform.forward, Loco.MoveOverrideType.Dash, powerDash);
    //     Vector2 knockBackDir2 = Utils.MakeV2(Br.myTransform.forward);
    //     knockBackDir2.Normalize();
    //     knockBackDir2 = Utils.RotateV2(knockBackDir2, 45f * (Random.Range(0,2) - 1));
    //     PassDataContainer container = new PassDataContainer()
    //     {
    //         data = new PassData[2]
    //         {
    //             new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { Br.character.GetStat(Stats.MeleeDamage) }),
    //             new PassDataKnockBack(30, knockBackDir2)
    //         }
    //     };
    //     SpellMain dash = Instantiate(GetSpellByAttackType(AnimAttackType.Ultimate), Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
    //     dash.transporter.target = Br.myTransform;
    //     dash.spell.areaOfEffect = Br.size + 1;
    //     dash.spell.lifeTime = Ga.me.gameData.dashTime;
    //     dash.InitializeMe(Br, container, afterSpellPush);
    //     return;
    //     
    //     void afterSpellPush()
    //     {
    //         PassDataContainer containerAfterSpellPush = new PassDataContainer()
    //         {
    //             data = new PassData[1]
    //             {
    //                 new PassDataKnockBack(30)
    //             }
    //         };
    //         SpellMain push = Instantiate(Ga.me.spells.push, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
    //         push.InitializeMe(Br, containerAfterSpellPush);
    //     }
    //
    // }

    public override void CombatEventRegistered(CombatEvent combatEvent, Brain otherBrain = null)
    {
        base.CombatEventRegistered(combatEvent, otherBrain);
        string st = otherBrain == null ? "" : $", target is {otherBrain.name}.";
//       print($"{combatEvent} {st}");
        switch (combatEvent)
        {
            case CombatEvent.Strike:
                // if (!Br.health.IsAtFullHealth()) return;
                // PassDataContainer containerArc = new PassDataContainer()
                // {
                //     canBeBlocked = true,
                //     data = new PassData[1]
                //     {
                //         new PassDataDamage(new Element[1] { Element.Magic }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                //     }
                // };
                // SpellMain arc = Instantiate(Ga.me.spells.sweepingArc, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                // arc.InitializeMe(Br, containerArc);
                break;
            case CombatEvent.Hit:
                break;
            case CombatEvent.Miss:
                float chance = 0.05f;
                if (Random.value > chance || Br.combat.MyTarget == null) return;
                Vector3 dir = Utils.Direction(myShield.position, Br.combat.MyTarget.position);
                PassDataContainer containerThrow = new PassDataContainer()
                {
                    canBeBlocked = true,
                    data = new PassData[1]
                    {
                        new PassDataDamage(new Element[1] { Element.Magic }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                    }
                };
                SpellMain shieldThrow = Instantiate(Ga.me.spells.shieldThrow, myShield.position, Quaternion.LookRotation(dir), Ga.me.spells.myTransform);
                shieldThrow.InitializeMe(Br, containerThrow);
                break;
            case CombatEvent.GetHit:
                break;
            case CombatEvent.Block:
                StartCoroutine(SpellPushDelay());
                break;
            case CombatEvent.Kill:
                if (otherBrain.character.GetStat(Stats.MagicDamage) >= Br.character.GetStat(Stats.MagicDamage))
                {
                    Br.character.ChangeStat(Character.BuffType.Skill,Stats.MagicDamage, 1);
                }
                break;
        }
        return;
        
        IEnumerator SpellPushDelay()
        {
            yield return new WaitForSeconds(0.1f);
            PassDataContainer container = new PassDataContainer()
            {
                myBrain = Br,
                data = new PassData[1]
                {
                    new PassDataKnockBack(30)
                }
            };
            SpellMain push = Instantiate(Ga.me.spells.push, myShield.position, Quaternion.identity, Ga.me.spells.myTransform);
            push.InitializeMe(Br, container);
        }
    }



}
