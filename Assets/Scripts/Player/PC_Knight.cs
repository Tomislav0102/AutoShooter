using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;


public class PC_Knight : MonoBehaviour, IIniBrain
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            // switch (startActive)
            // {
            //     case 0:
            //         SpellMain reflect = Instantiate(Ga.me.spells.reflectProjectile, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            //         reflect.transporter.target = value.myTransform;
            //         reflect.InitializeMe(value);
            //         break;
            //     case 1:
            //         SpellMain aura = Instantiate(Ga.me.spells.auraLowerAttSpeed, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform); 
            //         aura.transporter.target = value.myTransform;
            //         aura.InitializeMe(value);
            //         break;
            // }

        }
    }
    Brain _br;


    [SerializeField] Transform myShield;
    [SerializeField] int powerKnockback = 5;
    public int startActive;

    public void AnimEv_AttackCallback(int num = 0)
    {
        PassDataContainer container = new PassDataContainer()
        {
            canBeBlocked = true,
            data = new PassData[2]
            {
                new PassDataDamage(new Element[1] { Element.Magic }, new float[1] {Br.character.GetStat(Stats.MeleeDamage) }, true),
                new PassDataKnockBack(powerKnockback)
            }
        };
        
        SpellMain melee = Instantiate(Ga.me.spells.meleePlayer, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
        melee.onHitTarget += (Brain br) =>
        {
            if (br == null) Br.combat.CombatEventRegistered(CombatEvent.Miss);
            else Br.combat.CombatEventRegistered(CombatEvent.Hit, br);
        };
        melee.InitializeMe(Br, container);
    }
    
    public void AnimEv_UltimateCallback(int num = 0)
    {
       // healSpell();
       dashSpell();
        void dashSpell()
        {
            PassDataContainer pdDash = new PassDataContainer()
            {
                data = new PassData[1]
                {
                    new PassDataDash(Ga.me.gameData.dashPower),
                }
            };
            SpellMain dash = Instantiate(Ga.me.spells.dash,Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            dash.transporter.target = Br.myTransform;
            dash.lifeTime = Ga.me.gameData.dashTime;
            dash.InitializeMe(Br, pdDash);
            
            Vector2 knockBackDir2 = Utils.MakeV2(Br.myTransform.forward);
            knockBackDir2.Normalize();
            knockBackDir2 = Utils.RotateV2(knockBackDir2, 45f * (2 * Random.Range(0,2) - 1));
            PassDataContainer pdContactDamage = new PassDataContainer()
            {
                data = new PassData[2]
                {
                    new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { 4f }),
                    new PassDataKnockBack(powerKnockback, knockBackDir2)
                }
            };
            SpellMain contactDam = Instantiate(Ga.me.spells.contactDamage, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            contactDam.transporter.target = Br.myTransform;
            contactDam.lifeTime = Ga.me.gameData.dashTime;
            contactDam.areaOfEffect = 1.3f * Br.size;
            contactDam.InitializeMe(Br, pdContactDamage);
        }
        void healSpell()
        {
            SpellMain heal = Instantiate(Ga.me.spells.heal, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            heal.transporter.target = Br.myTransform;
            var containerHeal = new PassDataContainer()
            {
                data = new PassData[1]
                {
                    new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { -200f })
                }
            };
            heal.InitializeMe(Br, containerHeal);
        }
    }

    public void CombatEventCallback(CombatEvent combatEvent, Brain otherBrain = null)
    {
        return;
        string st = otherBrain == null ? "" : $", target is {otherBrain.name}.";
     //  print($"{combatEvent} {st}");
        switch (combatEvent)
        {
            case CombatEvent.Strike:
                if (!Br.health.IsAtFullHealth()) return;
                PassDataContainer containerArc = new PassDataContainer()
                {
                    canBeBlocked = true,
                    data = new PassData[1]
                    {
                        new PassDataDamage(new Element[1] { Element.Magic }, new float[1] {Br.character.GetStat(Stats.MagicDamage) }),
                    }
                };
                SpellMain arc = Instantiate(Ga.me.spells.sweepingArc,Br.myTransform.position,Br.myTransform.rotation, Ga.me.spells.myTransform);
                arc.InitializeMe(Br, containerArc);
                break;
            case CombatEvent.Hit:
                break;
            case CombatEvent.Miss:
                float chance = 0.05f;
                if (Random.value > chance ||Br.combat.MyTarget == null) return;
                Vector3 dir = Utils.Direction(myShield.position,Br.combat.MyTarget.position);
                PassDataContainer containerThrow = new PassDataContainer()
                {
                    canBeBlocked = true,
                    data = new PassData[1]
                    {
                        new PassDataDamage(new Element[1] { Element.Magic }, new float[1] {Br.character.GetStat(Stats.MagicDamage) }),
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
                // if (otherBrain.character.GetStat(Stats.MagicDamage) >=Br.character.GetStat(Stats.MagicDamage))
                // {
                //    Br.character.AddBuff(Stats.MagicDamage, 1);
                // }
                break;
        }
        return;
        
        IEnumerator SpellPushDelay()
        {
            yield return new WaitForSeconds(0.1f);
            PassDataContainer container = new PassDataContainer()
            {
                myBrain =Br,
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
