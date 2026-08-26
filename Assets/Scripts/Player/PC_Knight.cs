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
    public int startActive;
    float _consecutiveStrikeIncrease = 1f;
    MyDuo<Element, float> _elementStrikesIncrease = new MyDuo<Element, float>();
    
    public void AnimEv_AttackCallback(int num = 0)
    {
        MyDuo<Element, float> totalDamage = new MyDuo<Element, float>();
        totalDamage.Add(Element.Physical, Br.character.GetStat(Stats.MeleeDamage, _consecutiveStrikeIncrease));
        totalDamage.AddRange(_elementStrikesIncrease);
        PassDataContainer container = new PassDataContainer()
        {
            canBeBlocked = true,
            data = new PassData[2]
            {
                 new PassDataDamage(totalDamage, true),
                 new PassDataKnockBack(Br.character.GetStat(Stats.KnockBack))
            }
        };
        SpellMain melee = Instantiate(Br.character.skillPair.GetValue(0).spell, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
        melee.onHitTarget += (Brain br) =>
        {
            if (br == null) Br.combat.CombatEventRegistered(CombatEvent.Miss);
            else Br.combat.CombatEventRegistered(CombatEvent.Hit, br);
        };
        melee.InitializeMe(Br, container);
        _consecutiveStrikeIncrease = 1f;
        _elementStrikesIncrease = new MyDuo<Element, float>();
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
                    new PassDataKnockBack(Br.character.GetStat(Stats.KnockBack), knockBackDir2)
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
        string st = otherBrain == null ? "" : $", target is {otherBrain.name}.";
     //  print($"{combatEvent} {st}");
        switch (combatEvent)
        {
            case CombatEvent.Strike:
                int strikes = Br.combat.counterStrike;

                sweepingArc();
                consecutiveStrikes();
                elementalStrikes();

                void sweepingArc()
                {
                    if (!Br.health.IsAtFullHealth()) return;
                    if (!Br.character.skillPair.GetKey(1)) return;
                    PassDataContainer containerArc = new PassDataContainer()
                    {
                        canBeBlocked = true,
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Magic }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                        }
                    };
                    SpellMain arc = Instantiate(Br.character.skillPair.GetValue(1).spell, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                    arc.InitializeMe(Br, containerArc);
                }
                void consecutiveStrikes()
                {
                    if (!Br.character.skillPair.GetKey(4)) return;
                    int skillLevel = Br.character.skillPair.GetValue(4).level + 2;
                    int valueToAdd = 50;
                    int resultingAddedDamage = 0;

                    for (int i = 0; i < skillLevel; i++)
                    {
                        if (strikes % skillLevel != i) continue;
                        int val = valueToAdd * i - valueToAdd;
                        if (val < 0) val = (skillLevel - 1) * valueToAdd;
                        resultingAddedDamage += val;
                        _consecutiveStrikeIncrease = 1f + resultingAddedDamage * 0.01f;
                    }

                }
                void elementalStrikes()
                {
                    if (!Br.character.skillPair.GetKey(5)) return;
                    SoSkill skill =  Br.character.skillPair.GetValue(5);
                    _elementStrikesIncrease = new MyDuo<Element, float>();
                    switch (skill.level)
                    {
                        case 0:
                            if (strikes % 4 == 0) _elementStrikesIncrease.Add(skill.extraDamage.GetKey(0), skill.extraDamage.GetValue(0));
                            break;
                        case 1:
                            if (strikes % 4 == 0) _elementStrikesIncrease.Add(skill.extraDamage.GetKey(0), skill.extraDamage.GetValue(0));
                            if (strikes % 4 == 1) _elementStrikesIncrease.Add(skill.extraDamage.GetKey(1), skill.extraDamage.GetValue(1));
                            break;
                        case 2:
                            if (strikes % 4 == 0) _elementStrikesIncrease.Add(skill.extraDamage.GetKey(0), skill.extraDamage.GetValue(0));
                            if (strikes % 4 == 1) _elementStrikesIncrease.Add(skill.extraDamage.GetKey(1), skill.extraDamage.GetValue(1));
                            if (strikes % 4 == 2) _elementStrikesIncrease.Add(skill.extraDamage.GetKey(2), skill.extraDamage.GetValue(2));
                            break;
                        case 3:
                            if (strikes % 4 == 0) _elementStrikesIncrease.Add(skill.extraDamage.GetKey(0), skill.extraDamage.GetValue(0));
                            if (strikes % 4 == 1) _elementStrikesIncrease.Add(skill.extraDamage.GetKey(1), skill.extraDamage.GetValue(1));
                            if (strikes % 4 == 2) _elementStrikesIncrease.Add(skill.extraDamage.GetKey(2), skill.extraDamage.GetValue(2));
                            if (strikes % 4 == 3) _elementStrikesIncrease.Add(skill.extraDamage.GetKey(3), skill.extraDamage.GetValue(3));
                            break;
                    }
                }
                break;
            
            case CombatEvent.Hit:
                explosiveHit();
                void explosiveHit()
                {
                    if (!Br.character.skillPair.GetKey(3)) return;
                    if (Br.combat.counterHit % 3 != 0) return;
                    SpellMain ex = Instantiate(Br.character.skillPair.GetValue(3).spell, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    PassDataContainer container = new PassDataContainer()
                    {
                        canBeBlocked = true,
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { 100 * Br.character.GetStat(Stats.MagicDamage) }),
                        }
                    };
                    ex.transporter.target = Br.combat.MyTarget;
                    ex.InitializeMe(Br, container);
                }
                break;
            
            case CombatEvent.Miss:
                float chance = 0.05f;
                if (Random.value > chance || !Br.character.skillPair.GetKey(2)) return;
                SoSkill shieldThrowSkill = Br.character.skillPair.GetValue(2);
                
                Vector3 dir = Utils.Direction(myShield.position, Br.combat.MyTarget == null ? myShield.position + Br.myTransform.forward : Br.combat.MyTarget.position);
                int ricochet = Br.character.GetStat(Stats.Ricochet);
                MyDuo<Element, float> damage = new MyDuo<Element, float>();
                damage.Add(Element.Physical , Br.character.GetStat(Stats.RangedDamage) + shieldThrowSkill.extraDamage.GetValue(0));
                PassDataContainer containerThrow = new PassDataContainer()
                {
                    canBeBlocked = true,
                    data = new PassData[1]
                    {
                        new PassDataDamage(damage),
                    }
                };
                if (shieldThrowSkill.extraStats.TryGetValueByKey(Stats.Ricochet, out int extraStat))  ricochet += extraStat;
                
                SpellMain shieldThrow = Instantiate(Ga.me.spells.shieldThrow, myShield.position, Quaternion.LookRotation(dir), Ga.me.spells.myTransform);
                BulletTransporter bulletTransporter =  shieldThrow.transporter as BulletTransporter;
                bulletTransporter.ricochet = ricochet;
                shieldThrow.InitializeMe(Br, containerThrow);
                break;
            
            case CombatEvent.GetHit:
                break;
            case CombatEvent.Block:
                StartCoroutine(spellPushDelay());
                break;
            case CombatEvent.Kill:
                // if (otherBrain.character.GetStat(Stats.MagicDamage) >=Br.character.GetStat(Stats.MagicDamage))
                // {
                //    Br.character.AddBuff(Stats.MagicDamage, 1);
                // }
                break;
        }
        return;
        
        IEnumerator spellPushDelay()
        {
            yield return Ga.me.wait01;
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
