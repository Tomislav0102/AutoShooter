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
        }
    }
    Brain _br;
    [SerializeField] Transform myShield;

    float _crescendoStrikeIncrease = 1f;
    BuffStats _buffStatsCrescendoStrike, _buffStatsGuardMight, _buffStatsGuardValor;
    MyDuo<Element, float> _elementStrikesIncrease;
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
            case SkillName.IronMirror:
                ironMirror();
                break;
        }
        return;

        void lethargicDomain()
        {
            if (!Br.skills.TryGetFromGroup(SkillName.LethargicDomain, out SoSkill skill)) return;
            if (_passiveLethargicDomain != null) _passiveLethargicDomain.MyPhase = SpellMain.Phase.EndEnd;
            _passiveLethargicDomain = Instantiate(skill.spell, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            _passiveLethargicDomain.transporter.target = Br.myTransform;
            _passiveLethargicDomain.areaOfEffect += skill.passData.stats[0].buffStats.data.value;
            _passiveLethargicDomain.InitializeMe(Br);
        }
        void ironMirror()//spell has no effect (only visual), all logic is in skill
        {
            if (!Br.skills.TryGetFromGroup(SkillName.IronMirror, out SoSkill skill)) return;
            SpellMain spell = Instantiate(skill.spell, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            spell.transporter.target = Br.myTransform;
            spell.areaOfEffect = 2f + Br.size;
            spell.InitializeMe(Br);
            Br.character.BuffInjectData(skill.stats[0]);
        }
    }

    #region SKILLS
    void GuardiansMight()
    {
        if (!Br.skills.TryGetFromGroup(SkillName.GuardiansMight, out SoSkill skill)) return;
        Br.character.BuffInjectData(new StatsGroup(GenChange.Remove, _buffStatsGuardMight));

        _buffStatsGuardMight = new BuffStats(Stats.DamPhysical, Ga.me.team.TeamMemberCount(Faction.BadGuys) * skill.floatGeneric + 1f);
        Br.character.BuffInjectData(new StatsGroup(GenChange.Add, BuffType.Percentage, _buffStatsGuardMight));
    }
    void GuardiansValor()
    {
        if (!Br.skills.TryGetFromGroup(SkillName.GuardiansValor, out SoSkill skill)) return;
        Br.character.BuffInjectData(new StatsGroup(GenChange.Remove, _buffStatsGuardValor));

        _buffStatsGuardValor = new BuffStats(Stats.ResistPhysical, Ga.me.team.TeamMemberCount(Faction.BadGuys) * skill.floatGeneric + 1f);
        Br.character.BuffInjectData(new StatsGroup(GenChange.Add, BuffType.Percentage, _buffStatsGuardValor));

    }
    #endregion

    public void AnimEv_AttackCallback(int num = 0)
    {
        return;
        //damage
        MyDuo<Element, float> totalDamage = Br.character.GetDamage();
        if (Br.skills.TryGetFromGroup(SkillName.ElementalStrikes, out _)) totalDamage = Br.character.GetDamage(Element.Physical, 1f, _elementStrikesIncrease);

        //attack effects
        bool addEffect = false;
        BuffEffects executioner = null;
        BuffEffects bleeding = null;
        if (Br.skills.TryGetFromGroup(SkillName.Executioner, out SoSkill skillExe))
        {
            addEffect = true;
            executioner = skillExe.buffEffect[0];
        }
        if (Br.skills.TryGetFromGroup(SkillName.BleedingStrike, out SoSkill skillBleed))
        {
            addEffect = true;
            bleeding = skillBleed.buffEffect[0];
        }

        //stats
        bool addArmorBreaker = false;
        StatsGroup armorBreaker = null;
        if (Br.skills.TryGetFromGroup(SkillName.ArmorBreaker, out SoSkill skillArmorBreak))
        {
            float chance = skillArmorBreak.floatGeneric;
            if (chance < Random.value)
            {
                addArmorBreaker = true;
                armorBreaker = skillArmorBreak.stats[0];
            }
        }

        PassData passData = new PassData()
        {
            myBrain = Br,
            canBeBlocked = true,
            hasDamage = true,
            damagePair = totalDamage,
            hasKnockback = true,
            knockbackPower = Br.character.GetStat(Stats.KnockBack),
            hasEffect = addEffect,
            effects = new BuffEffects[2] { executioner, bleeding },
            hasStats = addArmorBreaker,
            stats = new StatsGroup[1]{ armorBreaker }
        };

        SpellMain melee = Instantiate(Br.skills.myBasic.spell, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
        melee.onHitTarget += (Brain br) =>
        {
            if (br == null) Br.combat.CombatEventRegistered(CombatEvent.Miss);
            else Br.combat.CombatEventRegistered(CombatEvent.Hit, br);
        };
        melee.InitializeMe(Br, passData);

        Br.character.BuffInjectData(new StatsGroup(GenChange.Remove, _buffStatsCrescendoStrike));
        _crescendoStrikeIncrease = 1f;
        _elementStrikesIncrease = null;
    }


    public void AnimEv_UltimateCallback(int num = 0)
    {
        // healSpell();
        dashSpell();
        return;

        void dashSpell()
        {
            MyDuo<Element, float> totalDamage = Br.character.GetDamage();
            SpellGroup.ComboDash(Br, totalDamage);
        }
        void healSpell()
        {
            SpellMain heal = Instantiate(Ga.me.spells.heal, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            heal.transporter.target = Br.myTransform;
            var containerHeal = new PassData()
            {
                myBrain = Br,
                hasDamage = true,
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
                    PassData containerArc = new PassData()
                    {
                        myBrain = Br,
                        canBeBlocked = true,
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Magic, 1f, sk.passData.damagePair),
                    };
                    SpellMain arc = Instantiate(sk.spell, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                    arc.areaOfEffect += sk.passData.stats[0].buffStats.data.value;
                    arc.InitializeMe(Br, containerArc);
                }
                void grandCrescendo()
                {
                    if (!Br.skills.TryGetFromGroup(SkillName.GrandCrescendo, out SoSkill sk)) return;
                    int effSkillLevel = sk.level + 2;
                    int valueToAdd = 50;
                    int resultingAddedDamage = 0;

                    for (int i = 0; i < effSkillLevel; i++)
                    {
                        if (strikes % effSkillLevel != i) continue;
                        int val = valueToAdd * i - valueToAdd;
                        if (val < 0) val = (effSkillLevel - 1) * valueToAdd;
                        resultingAddedDamage += val;
                        _crescendoStrikeIncrease = 1f + resultingAddedDamage * 0.01f;
                        _buffStatsCrescendoStrike = new BuffStats(Stats.DamPhysical, _crescendoStrikeIncrease);
                        Br.character.BuffInjectData(new StatsGroup(GenChange.Add, BuffType.Percentage, _buffStatsCrescendoStrike));
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
                    PassData container = new PassData()
                    {
                        myBrain = Br,
                        canBeBlocked = true,
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(new Element[2] { Element.Physical, Element.Fire }, 0.5f * (sk.level + 1))
                    };
                    SpellMain ex = Instantiate(sk.spell, Br.combat.MyTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                    ex.areaOfEffect += sk.level + 1;
                    ex.InitializeMe(Br, container);
                }
                void concussiveWave()
                {
                    if (!Br.skills.TryGetFromGroup(SkillName.ConcussiveWave, out SoSkill sk) || Random.value > sk.floatGeneric) return;
                    PassData container = new PassData()
                    {
                        myBrain = Br,
                        canBeBlocked = true,
                        hasDamage = true,
                        damagePair = Br.character.GetDamage()
                    };
                    SpellMain concussive = Instantiate(sk.spell, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    concussive.areaOfEffect += sk.level + 4;
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
                    PassData containerThrow = new PassData()
                    {
                        myBrain = Br,
                        canBeBlocked = true,
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Physical, sk.extraDamage.GetValue(0))
                    };
                    ricochet += (int)sk.passData.stats[0].buffStats.data.value;

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
                    if (otherBrain.character.GetStat(Stats.DamMagic) < Br.character.GetStat(Stats.DamMagic)) return;
                    Br.character.BuffInjectData(sk.stats[0]);
                }
                break;
        }

    }
}

