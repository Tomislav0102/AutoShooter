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
            _playerCombat = GetComponent<PlayerCombat>();
            value.skills.onSkillIncrease += SkillIncreaseCallback;
        }
    }
    Brain _br;
    [SerializeField] Transform myShield;
    PlayerCombat _playerCombat;
    float _stunAttacksDurationExtra; //applied on every stun effect 
    SoSkill _skillSeismicAnchorage;
    float _timerSeismicAnchorage;
    float _crescendoStrikeIncrease = 1f;
    BuffStats _buffStatsCrescendoStrike, _buffStatsGuardMight, _buffStatsGuardValor;
    MyDuo<Element, float> _elementStrikesIncrease;
    SpellMain _passiveLethargicDomain;
    SpellMain _spellAdvanceGuard;
    bool _canAdvanceGuard;

    void OnEnable()
    {
        Ga.OnBrainAddRemove += CallEvOnBrainAddRemove;
    }
    void OnDisable()
    {
        Ga.OnBrainAddRemove -= CallEvOnBrainAddRemove;
        Br.skills.onSkillIncrease -= SkillIncreaseCallback;
    }
    void CallEvOnBrainAddRemove(Brain brain, GenChange change)
    {
        GuardiansMight();
        GuardiansValor();
    }
    void Update()
    {
        seismicAnchorage();
        return;
        
        void seismicAnchorage()
        {
            if (_skillSeismicAnchorage is null) return;
            _timerSeismicAnchorage += Time.deltaTime;
            if (Br.loco.IsMoving) _timerSeismicAnchorage = 0f;
            if (_timerSeismicAnchorage < _skillSeismicAnchorage.valueGeneric) return;
            _timerSeismicAnchorage = 0f;
            SpellMain sa = SpellMain.Sp(_skillSeismicAnchorage.spell, Br);
            PassData passData = new PassData()
            {
                hasEffect = true,
                effects = new BuffEffects[1]
                {
                    new BuffEffects(Br, Status.Effect.Stunned, 1f, Ga.me.gameData.stunDurationBase + _stunAttacksDurationExtra) //value is increased by bonus from blunt weapon, default is 1
                }
            };
            sa.InitializeMe(Br, passData);
        }
    }


    void SkillIncreaseCallback(SoSkill newSkill)
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
            case SkillName.HeavyImpact:
                Ga.me.runData.enStunDamage.ChangeBuff(GenChange.Add, BuffType.Percentage, (int)newSkill.valueGeneric);
                break;
            case SkillName.SkullCracker:
                _stunAttacksDurationExtra += newSkill.valueGeneric;
                break;
            case SkillName.SeismicAnchorage:
                _skillSeismicAnchorage = newSkill;
                break;
            case SkillName.AdvanceGuard:
                _spellAdvanceGuard = SpellMain.Sp(newSkill.spell, Br, true);
                _spellAdvanceGuard.transporter.target = Br.myTransform;
                _spellAdvanceGuard.InitializeMe(Br);
                break;
            
            case SkillName.IronFortress: //ultimate 1
            case SkillName.ConcussiveSurge: //ultimate 2
                Br.skills.myUltimate = newSkill;
                Ga.me.uiManager.ultimateUi.SetMeUp(newSkill.valueGeneric);
                break;

                
        }
        return;

        void lethargicDomain()
        {
            if (!Br.skills.TryGetFromGroup(SkillName.LethargicDomain, out SoSkill skill)) return;
            if (_passiveLethargicDomain != null) _passiveLethargicDomain.MyPhase = SpellMain.Phase.EndEnd;
            _passiveLethargicDomain = SpellMain.Sp(skill.spell, Br);
            _passiveLethargicDomain.transporter.target = Br.myTransform;
            _passiveLethargicDomain.areaOfEffect += skill.passData.stats[0].data.value;
            _passiveLethargicDomain.InitializeMe(Br);
        }
        void ironMirror() //spell has no effect (only visual), all logic is in skill
        {
            if (!Br.skills.TryGetFromGroup(SkillName.IronMirror, out SoSkill skill)) return;
            SpellMain spell = SpellMain.Sp(skill.spell, Br);
            spell.transporter.target = Br.myTransform;
            spell.areaOfEffect = 2f + Br.size;
            spell.InitializeMe(Br);
            Br.character.CharacterInjectData(GenChange.Add, skill.passData.stats[0]);
        }
    }

    #region SKILLS
    void GuardiansMight()
    {
        if (!Br.skills.TryGetFromGroup(SkillName.GuardiansMight, out SoSkill skill)) return;
        Br.character.CharacterInjectData(GenChange.Remove, _buffStatsGuardMight);

        _buffStatsGuardMight = new BuffStats(Stats.DamPhysical, BuffType.Percentage,Ga.me.team.TeamMemberCount(Faction.BadGuys) * skill.valueGeneric + 1f);
        Br.character.CharacterInjectData(GenChange.Add, _buffStatsGuardMight);
    }
    void GuardiansValor()
    {
        if (!Br.skills.TryGetFromGroup(SkillName.GuardiansValor, out SoSkill skill)) return;
        Br.character.CharacterInjectData(GenChange.Remove, _buffStatsGuardValor);

        _buffStatsGuardValor = new BuffStats(Stats.ResistPhysical, BuffType.Percentage,Ga.me.team.TeamMemberCount(Faction.BadGuys) * skill.valueGeneric + 1f);
        Br.character.CharacterInjectData(GenChange.Add, _buffStatsGuardValor);
    }
    #endregion

    public void AnimEv_AttackCallback(int num = 0)
    {
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
            executioner = skillExe.passData.effects[0];
            executioner.myBrain = Br;
        }
        if (Br.skills.TryGetFromGroup(SkillName.BleedingStrike, out SoSkill skillBleed))
        {
            addEffect = true;
            bleeding = skillBleed.passData.effects[0];
            bleeding.myBrain = Br;
        }

        //stats
        bool addArmorBreaker = false;
        BuffStats armorBreaker = null;
        if (Br.skills.TryGetFromGroup(SkillName.ArmorBreaker, out SoSkill skillArmorBreak))
        {
            if (Random.value < skillArmorBreak.valueGeneric)
            {
                addArmorBreaker = true;
                armorBreaker = skillArmorBreak.passData.stats[0];
            }
        }

        PassData passData = new PassData()
        {
            hasDamage = true,
            damagePair = totalDamage,
            hasKnockback = true,
            knockbackPower = Br.character.GetStat(Stats.KnockBack),
            hasEffect = addEffect,
            effects = new BuffEffects[2] { executioner, bleeding },
            hasStats = addArmorBreaker,
            stats = new BuffStats[1] { armorBreaker }
        };

        SpellMain melee = SpellMain.Sp(Br.skills.myBasic.spell, Br, true);
        melee.InitializeMe(Br, passData);

        Br.character.CharacterInjectData(GenChange.Remove, _buffStatsCrescendoStrike);
        _crescendoStrikeIncrease = 1f;
        _elementStrikesIncrease = null;
    }

    public void AnimEv_UltimateCallback(int num = 0)
    {
        if (Br.skills.myUltimate is null) return;
        switch (Br.skills.myUltimate.skillName)
        {
            case SkillName.IronFortress: 
                ironFortress();
                void ironFortress()
                {
                    SpellMain spell = SpellMain.Sp(Br.skills.myUltimate.spell, Br);
                    spell.lifeTime = Br.skills.myUltimate.passData.stats[0].data.Duration;
                    spell.transporter.target = Br.myTransform;
                    spell.InitializeMe(Br);
                    for (int i = 0; i < Br.skills.myUltimate.passData.stats.Length; i++)
                    {
                        Br.character.CharacterInjectData(GenChange.Add, Br.skills.myUltimate.passData.stats[i]);
                    }
                    for (int i = 0; i < Br.skills.myUltimate.passData.effects.Length; i++)
                    {
                        Br.status.StatusInjectData(GenChange.Add, Br.skills.myUltimate.passData.effects[i]);
                    }
                }
                break;
            case SkillName.ConcussiveSurge:
                MyDuo<Element, float> totalDamage = Br.character.GetDamage();
                SpellGroup.ComboDash(Br, totalDamage);
                break;
        }

        void healSpell()
        {
            SpellMain heal = SpellMain.Sp(Ga.me.spells.heal, Br);
            heal.transporter.target = Br.myTransform;
            var containerHeal = new PassData()
            {
                hasDamage = true,
                damagePair = new MyDuo<Element, float>(new Element[1] { Element.Physical }, new float[1] { -200f })
            };
            heal.InitializeMe(Br, containerHeal);
        }
    }

    public void CombatEventCallback(CombatEvent combatEvent, Brain otherBrain = null, SpellMain.Specialty specialty = SpellMain.Specialty.General)
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
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Magic, 1f, sk.passData.damagePair),
                    };
                    SpellMain arc = SpellMain.Sp(sk.spell, Br, true);
                    arc.areaOfEffect += sk.passData.stats[0].data.value;
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
                        _buffStatsCrescendoStrike = new BuffStats(Stats.DamPhysical,BuffType.Percentage,  _crescendoStrikeIncrease);
                        Br.character.CharacterInjectData(GenChange.Add, _buffStatsCrescendoStrike);
                    }

                }
                void elementalStrikes()
                {
                    if (!Br.skills.TryGetFromGroup(SkillName.ElementalStrikes, out SoSkill sk)) return;
                    _elementStrikesIncrease = new MyDuo<Element, float>();
                    switch (sk.level)
                    {
                        case 0:
                            if (strikes % 4 == 0) _elementStrikesIncrease.Add(sk.passData.damagePair.GetKey(0), sk.passData.damagePair.GetValue(0));
                            break;
                        case 1:
                            if (strikes % 4 == 0) _elementStrikesIncrease.Add(sk.passData.damagePair.GetKey(0), sk.passData.damagePair.GetValue(0));
                            if (strikes % 4 == 1) _elementStrikesIncrease.Add(sk.passData.damagePair.GetKey(1), sk.passData.damagePair.GetValue(1));
                            break;
                        case 2:
                            if (strikes % 4 == 0) _elementStrikesIncrease.Add(sk.passData.damagePair.GetKey(0), sk.passData.damagePair.GetValue(0));
                            if (strikes % 4 == 1) _elementStrikesIncrease.Add(sk.passData.damagePair.GetKey(1), sk.passData.damagePair.GetValue(1));
                            if (strikes % 4 == 2) _elementStrikesIncrease.Add(sk.passData.damagePair.GetKey(2), sk.passData.damagePair.GetValue(2));
                            break;
                        case 3:
                            if (strikes % 4 == 0) _elementStrikesIncrease.Add(sk.passData.damagePair.GetKey(0), sk.passData.damagePair.GetValue(0));
                            if (strikes % 4 == 1) _elementStrikesIncrease.Add(sk.passData.damagePair.GetKey(1), sk.passData.damagePair.GetValue(1));
                            if (strikes % 4 == 2) _elementStrikesIncrease.Add(sk.passData.damagePair.GetKey(2), sk.passData.damagePair.GetValue(2));
                            if (strikes % 4 == 3) _elementStrikesIncrease.Add(sk.passData.damagePair.GetKey(3), sk.passData.damagePair.GetValue(3));
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
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(new Element[2] { Element.Physical, Element.Fire }, 1f, sk.passData.damagePair)
                    };
                    SpellMain ex = SpellMain.Sp(sk.spell, Br.combat.MyTarget);
                     ex.areaOfEffect += sk.level + 1;
                    ex.InitializeMe(Br, container);
                }
                void concussiveWave()
                {
                    if (!Br.skills.TryGetFromGroup(SkillName.ConcussiveWave, out SoSkill sk) || Random.value > sk.valueGeneric) return;
                    PassData container = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage()
                    };
                    SpellMain concussive = SpellMain.Sp(sk.spell, Br);
                    concussive.areaOfEffect += sk.level + 4;
                    concussive.InitializeMe(Br, container);
                }
                break;
            case CombatEvent.Miss:
                spectralRicochet();
                void spectralRicochet()
                {
                    if (!Br.skills.TryGetFromGroup(SkillName.SpectralRicochet, out SoSkill sk) || Random.value > sk.valueGeneric) return;
                    Vector3 dir = Utils.Direction(myShield.position, Br.combat.MyTarget == null ? myShield.position + Br.myTransform.forward : Br.combat.MyTarget.myTransform.position);
                    int ricochet = Br.character.GetStat(Stats.Ricochet);
                    PassData containerThrow = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Physical, 1f, sk.passData.damagePair)
                    };
                    ricochet += (int)sk.passData.stats[0].data.value;

                    SpellMain spell = SpellMain.Sp(sk.spell, Utils.LevelV3(myShield.position), Quaternion.LookRotation(dir));
                    spell.transporter.ricochet = ricochet;
                    spell.visual.SetSpawnHeight(myShield.position.y);
                    spell.InitializeMe(Br, containerThrow);
                }
                break;
            case CombatEvent.BeginGetHit:
                advanceGuard(true);
                break;
            case CombatEvent.GetHit:
                spikedRim();
                advanceGuard(false);
                void spikedRim()
                {
                    if (!(otherBrain is not null && specialty == SpellMain.Specialty.Melee)) return;
                    if (!Br.skills.TryGetFromGroup(SkillName.SpikedRim, out SoSkill skill)) return;
                    float damageValue = Br.character.GetStat(Stats.ResistPhysical) * skill.valueGeneric;
                    if (Mathf.Approximately(damageValue, 0f)) return;
                    PassData passData = new PassData()
                    {
                        hasDamage = true,
                        damagePair = new MyDuo<Element, float>(new Element[1] { Element.Physical }, new float[1] { damageValue })
                    };
                    otherBrain.health.HealthInjectDataDamage(passData, false, false);
                }
                break;
            case CombatEvent.EndGetHit:
                advanceGuard(false);
                break;
            case CombatEvent.Block:
                shockwaveBlock();
                void shockwaveBlock()
                {
                    if (!Br.skills.TryGetFromGroup(SkillName.ShockwaveBlock, out SoSkill sk)) return;
                    PassData passData = new PassData()
                    {
                        hasKnockback = true,
                        knockbackPower = Br.character.GetStat(Stats.KnockBack) + sk.passData.knockbackPower,
                    };
                    StartCoroutine(delay());
                    IEnumerator delay()
                    {
                        yield return Ga.me.wait01;
                        SpellMain push = SpellMain.Sp(sk.spell, Utils.LevelV3(myShield.position));
                        push.visual.SetSpawnHeight(myShield.position.y);
                        push.InitializeMe(Br, passData);
                    }
                }
                break;
            case CombatEvent.Kill:
                arcaneHarvest();
                void arcaneHarvest()
                {
                    if (!Br.skills.TryGetFromGroup(SkillName.ArcaneHarvest, out SoSkill sk)) return;
                    if (otherBrain.character.GetStat(Stats.DamMagic) < Br.character.GetStat(Stats.DamMagic)) return;
                    Br.character.CharacterInjectData(GenChange.Add, sk.passData.stats[0]);
                }
                break;
            
            
                void advanceGuard(bool beginPhase)
                {
                    if (otherBrain is null) return;
                    if (!Br.skills.TryGetFromGroup(SkillName.AdvanceGuard, out SoSkill skill)) return;
                    if (beginPhase)
                    {
                        if (!Br.loco.IsMoving) return;
                        if (Combat.IsFlanked(Br.myTransform, otherBrain.myTransform)) return;
                        _canAdvanceGuard = true;
                        _spellAdvanceGuard.visual.PlayDefault();
                        Br.character.CharacterInjectData(GenChange.Add, skill.passData.stats[0]);
                        return;
                    }
                    if (!_canAdvanceGuard) return;
                    _canAdvanceGuard = false;
                    Br.character.CharacterInjectData(GenChange.Remove, skill.passData.stats[0]);
                }

        }

    }
}

