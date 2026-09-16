using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
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
    Buff _buffCrescendoStrike, _buffGuardMight, _buffGuardValor;
    MyDuo<Element, float> _elementStrikesIncrease = new MyDuo<Element, float>();
    SpellMain _passiveLethargicDomain;
    

    void OnEnable()
    {
        EventBus.OnBrainAddRemove += CallEv_OnBrainAddRemove;
    }
    void OnDisable()
    {
        EventBus.OnBrainAddRemove -= CallEv_OnBrainAddRemove;
    }
    void CallEv_OnBrainAddRemove(Brain brain, GenChange change)
    {
        GuardiansMight();
        GuardiansValor();
    }

    public void SkillIncreaseCallback(SoSkill newSkill)
    {
        switch (newSkill.skillName)
        {
            case SkillName.GuardiansMight:
                GuardiansMight();
                break;
            case SkillName.GuardiansValor:
                GuardiansValor();
                break;
            case SkillName.LethargicDomain:
                lethargicDomain();
                break;
        }
        return;
        
        void lethargicDomain()
        {
            if (!Br.skills.TryGetFromGroup(SkillName.LethargicDomain, out SoSkill skill)) return;
            if (_passiveLethargicDomain != null) _passiveLethargicDomain.MyPhase = SpellMain.Phase.EndEnd;
            _passiveLethargicDomain = Instantiate(skill.spell, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            _passiveLethargicDomain.transporter.target = Br.myTransform;
            _passiveLethargicDomain.areaOfEffect += skill.passData.stats[0].buff.value;
            _passiveLethargicDomain.InitializeMe(Br);
        }

    }

    #region SKILLS
    void GuardiansMight()
    {
        if (!Br.skills.TryGetFromGroup(SkillName.GuardiansMight, out SoSkill skill)) return;
        Br.character.BuffInjectData(new StatsGroup(GenChange.Remove, _buffGuardMight));

        _buffGuardMight = new Buff(Stats.ExtraDamPhysical,  Ga.me.team.TeamMemberCount(Faction.BadGuys) * skill.floatGeneric + 1f);
        Br.character.BuffInjectData(new StatsGroup(GenChange.Add,BuffType.Percentage,  _buffGuardMight));
    }
    void GuardiansValor()
    {
        if (!Br.skills.TryGetFromGroup(SkillName.GuardiansValor, out SoSkill skill)) return;
        Br.character.BuffInjectData(new StatsGroup(GenChange.Remove, _buffGuardValor));

        _buffGuardValor = new Buff(Stats.ResistancePhysical, Ga.me.team.TeamMemberCount(Faction.BadGuys) * skill.floatGeneric + 1f);
        Br.character.BuffInjectData(new StatsGroup(GenChange.Add, BuffType.Percentage,_buffGuardValor));

    }
    #endregion
    
    public void AnimEv_AttackCallback(int num = 0)
    {
        return;
        //damage
        MyDuo<Element, float> totalDamage = Br.character.GetDamage(Stats.MeleeDamage);
        if (Br.skills.TryGetFromGroup(SkillName.ElementalStrikes, out _)) totalDamage.AddRange(_elementStrikesIncrease);

        //attack effects
        bool addEffect = false;
        EffectGroup executioner = null;
        EffectGroup bleeding = null;
        if (Br.skills.TryGetFromGroup(SkillName.Executioner, out SoSkill skillExe))
        {
            addEffect = true;
            executioner = skillExe.effects[0];
        }
        if (Br.skills.TryGetFromGroup(SkillName.BleedingStrike, out SoSkill skillBleed))
        {
            addEffect = true;
            bleeding = skillBleed.effects[0];
        }
        
        //stats
        bool addArmorBreaker = false;
        StatsGroup[] armorBreaker = null;
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
            myBrain = Br,
            canBeBlocked = true,
            hasDamage =  true,
            damagePair = totalDamage,
            hasKnockback = true,
            knockbackPower = Br.character.GetStat(Stats.KnockBack),
            hasEffect = addEffect,
            effects = new EffectGroup[2]
            {
                executioner,
                bleeding
            },
            hasStats = addArmorBreaker,
            stats = armorBreaker
        };
        
        SpellMain melee = Instantiate(Br.skills.myBasic.spell, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
        melee.onHitTarget += (Brain br) =>
        {
            if (br == null) Br.combat.CombatEventRegistered(CombatEvent.Miss);
            else Br.combat.CombatEventRegistered(CombatEvent.Hit, br);
        };
        melee.InitializeMe(Br, container);
        
        Br.character.BuffInjectData(new StatsGroup(GenChange.Remove, _buffCrescendoStrike));
        _crescendoStrikeIncrease = 1f;
        _elementStrikesIncrease = new MyDuo<Element, float>();
    }


    public void AnimEv_UltimateCallback(int num = 0)
    {
        // healSpell();
        dashSpell();
        return;

        void dashSpell()
        {
            MyDuo<Element, float> totalDamage = Br.character.GetDamage(Stats.MeleeDamage);
            SpellGroup.ComboDash(Br,  totalDamage);
        }
        void healSpell()
        {
            SpellMain heal = Instantiate(Ga.me.spells.heal, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            heal.transporter.target = Br.myTransform;
            var containerHeal = new PassDataContainer()
            {
                myBrain = Br,
                hasDamage =  true,
                damagePair = new MyDuo<Element, float>(new Element[1] { Element.Physical }, new float[1] { -200f })
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
                     myBrain = Br,
                     canBeBlocked = true,
                     hasDamage =  true,
                     damagePair = Br.character.GetDamage(Stats.MagicDamage),
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
                     _buffCrescendoStrike = new Buff(Stats.MeleeDamage, _crescendoStrikeIncrease);
                     Br.character.BuffInjectData(new StatsGroup(GenChange.Add, BuffType.Percentage, _buffCrescendoStrike));
                 }

             }
             void elementalStrikes()
             {
                 if (!Br.skills.TryGetFromGroup(SkillName.ElementalStrikes, out SoSkill sk)) return;
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
                     myBrain = Br,
                     canBeBlocked = true,
                     hasDamage =  true,
                     damagePair = Br.character.GetDamage(Stats.MagicDamage)
                 };
                 SpellMain ex = Instantiate(sk.spell, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                 ex.transporter.target = Br.combat.MyTarget;
                 ex.InitializeMe(Br, container);
             }
             void concussiveWave()
             {
                 if (!Br.skills.TryGetFromGroup(SkillName.ConcussiveWave, out SoSkill sk) || Random.value > sk.floatGeneric) return;
                 PassDataContainer container = new PassDataContainer()
                 {
                     myBrain = Br,
                     canBeBlocked = true,
                     hasDamage =  true,
                     damagePair = Br.character.GetDamage(Stats.MeleeDamage)
                 };
                 SpellMain concussive = Instantiate(sk.spell, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                 concussive.areaOfEffect += sk.passData.stats[0].buff.value;
                 concussive.InitializeMe(Br, container);
             }
             break;
         case CombatEvent.Miss:
             spectralRicochet();
             void spectralRicochet()
             {
                 if (!Br.skills.TryGetFromGroup(SkillName.SpectralRicochet, out SoSkill sk) || Random.value > sk.floatGeneric) return;
                 Vector3 dir = Utils.Direction(myShield.position, Br.combat.MyTarget == null ? myShield.position + Br.myTransform.forward : Br.combat.MyTarget.position);
                 int ricochet = Br.character.GetStat(Stats.Ricochet);
                 MyDuo<Element, float> damage = new MyDuo<Element, float>();
                 damage.Add(Element.Physical, Br.character.GetStat(Stats.RangedDamage) + sk.extraDamage.GetValue(0));
                 PassDataContainer containerThrow = new PassDataContainer()
                 {
                     myBrain = Br,
                     canBeBlocked = true,
                     hasDamage =  true,
                     damagePair = Br.character.GetDamage(Stats.RangedDamage, sk.extraDamage)
                 };
                 ricochet += (int)sk.passData.stats[0].buff.value;
                 
                 SpellMain spell = Instantiate(sk.spell, Utils.MakeV2(myShield.position), Quaternion.LookRotation(dir), Ga.me.spells.myTransform);
                 BulletTransporter bulletTransporter = spell.transporter as BulletTransporter;
                 bulletTransporter.ricochet = ricochet;
                 spell.visual.SetSpawnHeight(myShield.position.y);
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
                     push.InitializeMe(Br, sk.passData);
                 }
             }
             break;
         case CombatEvent.Kill:
             arcaneHarvest();
             void arcaneHarvest()
             {
                 if (!Br.skills.TryGetFromGroup(SkillName.ArcaneHarvest, out SoSkill sk)) return;
                 if (otherBrain.character.GetStat(Stats.MagicDamage) < Br.character.GetStat(Stats.MagicDamage)) return;
                 Br.character.BuffInjectData(sk.stats[0]);
             }
             break;

             void spikedRim()
             {
                 if (!Br.skills.TryGetFromGroup(SkillName.SpikedRim, out SoSkill sk)) return;
             }

     }

    }
}

