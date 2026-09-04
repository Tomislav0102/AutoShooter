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
    float _crescendoStrikeIncrease = 1f;
    MyDuo<Element, float> _elementStrikesIncrease = new MyDuo<Element, float>();
    SoSkill _myBasic;
    SoSkill[] _allSkills;
    
    public void SkillUpdate()
    {
        _allSkills = Br.skills.CurrentSkills();
        foreach (SoSkill item in _allSkills)
        {
            if (item.skillName != SkillName.KnightBase) continue;
            Br.skills.LevelSpecificSkill(SkillName.KnightBase, 0);
            _myBasic = item;
            break;
        }
    }

    
    public void AnimEv_AttackCallback(int num = 0)
    {
        //damage
        MyDuo<Element, float> totalDamage = new MyDuo<Element, float>(true);
        totalDamage.Add(Element.Physical, Br.character.GetStat(Stats.MeleeDamage, _crescendoStrikeIncrease));
        totalDamage.AddRange(_elementStrikesIncrease);

        //attack effects
        bool addEffect = false;
        PassDataEffect.Group executioner = null;
        PassDataEffect.Group bleeding = null;
        if (Br.skills.TryGetFromGroup(SkillName.Executioner, out SoSkill skillExe))
        {
            addEffect = true;
            executioner = skillExe.effect.group[0];
        }
        if (Br.skills.TryGetFromGroup(SkillName.BleedingStrike, out SoSkill skillBleed))
        {
            addEffect = true;
            bleeding = skillBleed.effect.group[0];
        }
        
        //stats
        bool addArmorBreaker = false;
        PassDataStats.Group[] armorBreaker = null;
        if (Br.skills.TryGetFromGroup(SkillName.ArmorBreaker, out SoSkill skillArmorBreak))
        {
            float chance = 0.05f;
            if (chance < Random.value)
            {
                addArmorBreaker = true;
                armorBreaker = skillArmorBreak.stats;
            }
        }
        
        PassDataContainer container = new PassDataContainer()
        {
            canBeBlocked = true,
            data = new List<PassData>()
            {
                 new PassDataDamage(totalDamage, true),
                 new PassDataKnockBack(Br.character.GetStat(Stats.KnockBack)),
            }
        };
        if (addEffect) container.data.Add(new PassDataEffect() { group = new PassDataEffect.Group[2] {executioner, bleeding}});
        if (addArmorBreaker) container.data.Add(new PassDataStats() { group = armorBreaker });
        
        SpellMain melee = Instantiate(_myBasic.spell, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
        melee.onHitTarget += (Brain br) =>
        {
            if (br == null) Br.combat.CombatEventRegistered(CombatEvent.Miss);
            else Br.combat.CombatEventRegistered(CombatEvent.Hit, br);
        };
        melee.InitializeMe(Br, container);
        
        _crescendoStrikeIncrease = 1f;
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
                data = new List<PassData>()
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
                data = new List<PassData>()
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
                data = new List<PassData>()
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
             grandCrescendo();
             elementalStrikes();

             void sweepingArc()
             {
                 if (!Br.health.IsAtFullHealth()) return;
                 if (!Br.skills.TryGetFromGroup(SkillName.SweepingArc, out SoSkill sk)) return;
                 PassDataContainer containerArc = new PassDataContainer()
                 {
                     canBeBlocked = true,
                     data = new List<PassData>()
                     {
                         new PassDataDamage(new Element[1] { Element.Magic }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                     }
                 };
                 SpellMain arc = Instantiate(sk.spell, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                 arc.InitializeMe(Br, containerArc);
             }
             void grandCrescendo()
             {
                 if (!Br.skills.TryGetFromGroup(SkillName.GrandCrescendo, out SoSkill sk)) return;
                 int skillLevel = sk.level + 2;
                 int valueToAdd = 50;
                 int resultingAddedDamage = 0;

                 for (int i = 0; i < skillLevel; i++)
                 {
                     if (strikes % skillLevel != i) continue;
                     int val = valueToAdd * i - valueToAdd;
                     if (val < 0) val = (skillLevel - 1) * valueToAdd;
                     resultingAddedDamage += val;
                     _crescendoStrikeIncrease = 1f + resultingAddedDamage * 0.01f;
                 }

             }
             void elementalStrikes()
             {
                 if (!Br.skills.TryGetFromGroup(SkillName.GrandCrescendo, out SoSkill sk)) return;
                 _elementStrikesIncrease = new MyDuo<Element, float>();
                 switch (sk.level)
                 {
                     case 0:
                         if (strikes % 4 == 0) _elementStrikesIncrease.Add(sk.extraDamage.GetKey(0), sk.extraDamage.GetValue(0));
                         break;
                     case 1:
                         if (strikes % 4 == 0) _elementStrikesIncrease.Add(sk.extraDamage.GetKey(0), sk.extraDamage.GetValue(0));
                         if (strikes % 4 == 1) _elementStrikesIncrease.Add(sk.extraDamage.GetKey(1), sk.extraDamage.GetValue(1));
                         break;
                     case 2:
                         if (strikes % 4 == 0) _elementStrikesIncrease.Add(sk.extraDamage.GetKey(0), sk.extraDamage.GetValue(0));
                         if (strikes % 4 == 1) _elementStrikesIncrease.Add(sk.extraDamage.GetKey(1), sk.extraDamage.GetValue(1));
                         if (strikes % 4 == 2) _elementStrikesIncrease.Add(sk.extraDamage.GetKey(2), sk.extraDamage.GetValue(2));
                         break;
                     case 3:
                         if (strikes % 4 == 0) _elementStrikesIncrease.Add(sk.extraDamage.GetKey(0), sk.extraDamage.GetValue(0));
                         if (strikes % 4 == 1) _elementStrikesIncrease.Add(sk.extraDamage.GetKey(1), sk.extraDamage.GetValue(1));
                         if (strikes % 4 == 2) _elementStrikesIncrease.Add(sk.extraDamage.GetKey(2), sk.extraDamage.GetValue(2));
                         if (strikes % 4 == 3) _elementStrikesIncrease.Add(sk.extraDamage.GetKey(3), sk.extraDamage.GetValue(3));
                         break;
                 }
             }
             break;

         case CombatEvent.Hit:
             explosiveHit();
             concussiveWave();
             void explosiveHit()
             {
                 if (!Br.skills.TryGetFromGroup(SkillName.ExplosiveHit, out SoSkill sk)) return;
                 if (Br.combat.counterHit % 3 != 0) return;
                 PassDataContainer container = new PassDataContainer()
                 {
                     canBeBlocked = true,
                     data = new List<PassData>()
                     {
                         new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                     }
                 };
                 SpellMain ex = Instantiate(sk.spell, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                 ex.transporter.target = Br.combat.MyTarget;
                 ex.InitializeMe(Br, container);
             }
             void concussiveWave()
             {
                 if (!Br.skills.TryGetFromGroup(SkillName.ConcussiveWave, out SoSkill sk)) return;
                 if (sk.floatGeneric < Random.value) return;
                 PassDataContainer container = new PassDataContainer()
                 {
                     canBeBlocked = true,
                     data = new List<PassData>()
                     {
                         new PassDataDamage(new Element[1] { Element.Magic }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                     }
                 };
                 SpellMain concussive = Instantiate(sk.spell, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                 concussive.areaOfEffect += sk.block.stats.group[0].value;
                 concussive.InitializeMe(Br, container);
             }
             break;

         case CombatEvent.Miss:
             spectralRicochet();
             void spectralRicochet()
             {
                 float chance = 0.05f;
                 if (Random.value > chance || !Br.skills.TryGetFromGroup(SkillName.SpectralRicochet, out SoSkill sk)) return;

                 Vector3 dir = Utils.Direction(myShield.position, Br.combat.MyTarget == null ? myShield.position + Br.myTransform.forward : Br.combat.MyTarget.position);
                 int ricochet = Br.character.GetStat(Stats.Ricochet);
                 MyDuo<Element, float> damage = new MyDuo<Element, float>();
                 damage.Add(Element.Physical, Br.character.GetStat(Stats.RangedDamage) + sk.extraDamage.GetValue(0));
                 PassDataContainer containerThrow = new PassDataContainer()
                 {
                     canBeBlocked = true,
                     data = new List<PassData>()
                     {
                         new PassDataDamage(damage),
                     }
                 };
                 ricochet += sk.block.stats.group[0].value;

                 SpellMain spell = Instantiate(sk.spell, myShield.position, Quaternion.LookRotation(dir), Ga.me.spells.myTransform);
                 BulletTransporter bulletTransporter = spell.transporter as BulletTransporter;
                 bulletTransporter.ricochet = ricochet;
                 spell.InitializeMe(Br, containerThrow);
             }
             break;

         case CombatEvent.GetHit:
             break;
         case CombatEvent.Block: 
             shockwaveBlock();
             void shockwaveBlock()
             {
                 if (!Br.skills.TryGetFromGroup(SkillName.ShockwaveBlock, out SoSkill sk)) return;
                 StartCoroutine(delay());
                 IEnumerator delay()
                 {
                     yield return Ga.me.wait01;
                     SpellMain push = Instantiate(sk.spell, myShield.position, Quaternion.identity, Ga.me.spells.myTransform);
                     push.InitializeMe(Br, sk.block.GetContainer());
                 }
             }
             break;
         case CombatEvent.Kill:
             arcaneHarvest();
             void arcaneHarvest()
             {
                 if (!Br.skills.TryGetFromGroup(SkillName.ArcaneHarvest, out SoSkill sk)) return;
                 if (otherBrain.character.GetStat(Stats.MagicDamage) < Br.character.GetStat(Stats.MagicDamage)) return;
                 PassDataStats pds = new PassDataStats()
                 {
                     group = sk.stats
                 };
                 Br.character.BuffInjectData(pds);
             }
             break;
     }

    }
}
