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
    float _stunAttacksDurationExtra;

    #region DATA
    SoSkill _skillSweepingArc;
    SoSkill _skillGrandCrescendo;
    SoSkill _skillExplosiveHit;
    SoSkill _skillConcussiveWave;
    SoSkill _skillSpectralRicochet;
    SoSkill _skillSpikedRim;
    SoSkill _skillSkullCracker;
    SoSkill _skillShockwaveBlock;
    SoSkill _skillArcaneHarvest;
    SoSkill _skillExecutioner;
    SoSkill _skillBleedingStrike;
    SoSkill _skillArmorBreaker;
    SoSkill _skillElementalStrikes;
    SoSkill _skillGuardiansMight;
    SoSkill _skillGuardiansValor;
    SoSkill _skillHeavyImpact;
    SoSkill _skillAdvanceGuard;
    SpellMain _spellAdvanceGuard;
    SoSkill _skillStatue;
    bool _canStatueSpawn = true;
    SoSkill _skillIronMirror;
    SpellMain _spellIronMirror;
    SoSkill _skillLethargicDomain;
    SpellMain _spellLethargicDomain;
    SoSkill _skillSeismicAnchorage;
    float _timerSeismicAnchorage;
    float _crescendoStrikeIncrease = 1f;
    BuffStats _buffStatsCrescendoStrike, _buffStatsGuardMight, _buffStatsGuardValor;
    MyDuo<Element, float> _damElementStrikesIncrease;
    bool _canAdvanceGuard;
    Queue<SpellMain> _queueStatues;
    int _numMaxStatues;
    SoSkill _skillFuneralTornado;
    float _cooldownFuneralTornado;
    const int CONST_CooldownFuneralTornado = 4;
    #endregion

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
                    new BuffEffects(Status.Effect.Stunned, 1f, Ga.me.gameData.stunDurationBase + _stunAttacksDurationExtra, Br) //value is increased by bonus from blunt weapon, default is 1
                }
            };
            sa.InitializeMe(Br, passData);
        }
    }


    void SkillIncreaseCallback(SoSkill newSkill)
    {
        switch (newSkill.skillId)
        {
            case SkillId.SweepingArc:
                _skillSweepingArc = newSkill;
                break;
            case SkillId.GuardiansMight:
                _skillGuardiansMight = newSkill;
                GuardiansMight();
                break;
            case SkillId.GuardiansValor:
                _skillGuardiansValor =  newSkill;
                GuardiansValor();
                break;
            case SkillId.LethargicDomain:
                _skillLethargicDomain = newSkill;
                lethargicDomain();
                break;
            case SkillId.IronMirror:
                _skillIronMirror =  newSkill;
                ironMirror();
                break;
            case SkillId.HeavyImpact:
                _skillHeavyImpact = newSkill;
                Ga.me.runData.AddAllVulnerabilities(Status.Effect.Stunned, (int)_skillHeavyImpact.valueGeneric);
                break;
            case SkillId.SkullCracker:
                _skillSkullCracker = newSkill;
                _stunAttacksDurationExtra += _skillSkullCracker.valueGeneric;
                break;
            case SkillId.AdvanceGuard:
                _skillAdvanceGuard = newSkill;
                _spellAdvanceGuard = SpellMain.Sp(_skillAdvanceGuard.spell, Br, true);
                _spellAdvanceGuard.transporter.target = Br.myTransform;
                _spellAdvanceGuard.InitializeMe(Br);
                break;
            case SkillId.Statue:
                _skillStatue = newSkill;
                _queueStatues = new Queue<SpellMain>();
                _numMaxStatues++;
                break;
            case SkillId.GrandCrescendo:
                _skillGrandCrescendo = newSkill;
                break;
            case SkillId.ExplosiveHit:
                _skillExplosiveHit = newSkill;
                break;
            case SkillId.ConcussiveWave:
                _skillConcussiveWave = newSkill;
                break;
            case SkillId.SpectralRicochet:
                _skillSpectralRicochet = newSkill;
                break;
            case SkillId.SpikedRim:
                _skillSpikedRim = newSkill;
                break;
            case SkillId.ShockwaveBlock:
                _skillShockwaveBlock = newSkill;
                break;
            case SkillId.ArcaneHarvest:
                _skillArcaneHarvest = newSkill;
                break;
            case SkillId.Executioner:
                _skillExecutioner = newSkill;
                break;
            case SkillId.BleedingStrike:
                _skillBleedingStrike = newSkill;
                break;
            case SkillId.ArmorBreaker:
                _skillArmorBreaker = newSkill;
                break;
            case SkillId.ElementalStrikes:
                _skillElementalStrikes = newSkill;
                break;
            case SkillId.FuneralTornado:
                _skillFuneralTornado = newSkill;
                break;
            case SkillId.IronFortress: //ultimate 1
            case SkillId.ConcussiveSurge: //ultimate 2
                Br.skills.myUltimate = newSkill;
                Ga.me.uiManager.ultimateUi.SetMeUp(newSkill.valueGeneric);
                break;

                
        }
        return;

        void lethargicDomain()
        {
            if (_skillLethargicDomain is null) return;
            if (_spellLethargicDomain is not null) _spellLethargicDomain.MyPhase = SpellMain.Phase.EndEnd;
            _spellLethargicDomain = SpellMain.Sp(_skillLethargicDomain.spell, Br);
            _spellLethargicDomain.transporter.target = Br.myTransform;
            _spellLethargicDomain.areaOfEffect += _skillLethargicDomain.passData.stats[0].value;
            PassData pd = new PassData()
            {
                hasStats = true,
                stats = new BuffStats[1] { _skillLethargicDomain.passData.stats[1] }
            };
            _spellLethargicDomain.InitializeMe(Br, pd);
        }
        void ironMirror() //spell has no effect (only visual), all logic is in skill
        {
            if (_skillIronMirror is null) return;
            if (_spellIronMirror is not null) _spellIronMirror.MyPhase = SpellMain.Phase.EndEnd;
            _spellIronMirror = SpellMain.Sp(_skillIronMirror.spell, Br);
            _spellIronMirror.transporter.target = Br.myTransform;
            _spellIronMirror.areaOfEffect = 2f + Br.character.GetStat(Stats.BodySize) * 0.01f;
            _spellIronMirror.InitializeMe(Br);
            Br.character.CharacterInjectData(GenChange.Add, _skillIronMirror.passData.stats[0]);
        }
    }

    #region SKILLS
    void GuardiansMight()
    {
        if (_skillGuardiansMight is null) return;
        Br.character.CharacterInjectData(GenChange.Remove, _buffStatsGuardMight);

        _buffStatsGuardMight = new BuffStats(Stats.DamPhysical, BuffType.Added,Ga.me.team.TeamMemberCount(Faction.BadGuys) * _skillGuardiansMight.valueGeneric);
        Br.character.CharacterInjectData(GenChange.Add, _buffStatsGuardMight);
    }
    void GuardiansValor()
    {
        if (_skillGuardiansValor is null) return;
        Br.character.CharacterInjectData(GenChange.Remove, _buffStatsGuardValor);

        _buffStatsGuardValor = new BuffStats(Stats.ResistPhysical, BuffType.Added,Ga.me.team.TeamMemberCount(Faction.BadGuys) * _skillGuardiansValor.valueGeneric);
        Br.character.CharacterInjectData(GenChange.Add, _buffStatsGuardValor);
    }
    #endregion

    public void AnimEv_AttackCallback(int num = 0)
    {
        //damage
        MyDuo<Element, float> totalDamage = Br.character.GetDamage();
        if (_skillElementalStrikes is not null) totalDamage = Br.character.GetDamage(Element.Physical, 1f, _damElementStrikesIncrease);

        //attack effects
        bool addEffect = false;
        BuffEffects executioner = null;
        BuffEffects bleeding = null;
        if (_skillExecutioner is not null)
        {
            addEffect = true;
            executioner = _skillExecutioner.passData.effects[0];
            executioner.myBrain = Br;
        }
        if (_skillBleedingStrike is not null)
        {
            addEffect = true;
            bleeding = _skillBleedingStrike.passData.effects[0];
            bleeding.myBrain = Br;
        }

        //stats
        bool addArmorBreaker = false;
        BuffStats armorBreaker = null;
        if (_skillArmorBreaker is not null)
        {
            if (Random.value < _skillArmorBreaker.valueGeneric)
            {
                addArmorBreaker = true;
                armorBreaker = _skillArmorBreaker.passData.stats[0];
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
        melee.range += Br.character.GetStat(Stats.Size) * 0.01f;
        melee.InitializeMe(Br, passData);

        Br.character.CharacterInjectData(GenChange.Remove, _buffStatsCrescendoStrike);
        _crescendoStrikeIncrease = 1f;
        _damElementStrikesIncrease = null;
    }

    public void AnimEv_UltimateCallback(int num = 0)
    {
        if (Br.skills.myUltimate is null) return;
        switch (Br.skills.myUltimate.skillId)
        {
            case SkillId.IronFortress: 
                ironFortress();
                void ironFortress()
                {
                    SpellMain spell = SpellMain.Sp(Br.skills.myUltimate.spell, Br);
                    spell.lifeTime = Br.skills.myUltimate.passData.stats[0].Duration;
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
            case SkillId.ConcussiveSurge:
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
                    if (_skillSweepingArc is null) return;
                    if (!Br.health.IsAtFullHealth()) return;
                    PassData containerArc = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Magic, 1f, _skillSweepingArc.passData.damagePair),
                    };
                    SpellMain arc = SpellMain.Sp(_skillSweepingArc.spell, Br, true);
                    arc.areaOfEffect += _skillSweepingArc.passData.stats[0].value;
                    arc.InitializeMe(Br, containerArc);
                }
                void grandCrescendo()
                {
                    if (_skillGrandCrescendo is null) return;
                    int effSkillLevel = _skillGrandCrescendo.level + 2;
                    int valueToAdd = 50;
                    int resultingAddedDamage = 0;

                    for (int i = 0; i < effSkillLevel; i++)
                    {
                        if (strikes % effSkillLevel != i) continue;
                        int val = valueToAdd * i - valueToAdd;
                        if (val < 0) val = (effSkillLevel - 1) * valueToAdd;
                        resultingAddedDamage += val;
                        _crescendoStrikeIncrease = 1f + resultingAddedDamage * 0.01f;
                        _buffStatsCrescendoStrike = new BuffStats(Stats.DamPhysical,BuffType.Added,  _crescendoStrikeIncrease);
                        Br.character.CharacterInjectData(GenChange.Add, _buffStatsCrescendoStrike);
                    }

                }
                void elementalStrikes()
                {
                    if (_skillElementalStrikes is null) return;
                    _damElementStrikesIncrease = new MyDuo<Element, float>();
                    switch (_skillElementalStrikes.level)
                    {
                        case 0:
                            if (strikes % 4 == 0) _damElementStrikesIncrease.Add(_skillElementalStrikes.passData.damagePair.GetKey(0), _skillElementalStrikes.passData.damagePair.GetValue(0));
                            break;
                        case 1:
                            if (strikes % 4 == 0) _damElementStrikesIncrease.Add(_skillElementalStrikes.passData.damagePair.GetKey(0), _skillElementalStrikes.passData.damagePair.GetValue(0));
                            if (strikes % 4 == 1) _damElementStrikesIncrease.Add(_skillElementalStrikes.passData.damagePair.GetKey(1), _skillElementalStrikes.passData.damagePair.GetValue(1));
                            break;
                        case 2:
                            if (strikes % 4 == 0) _damElementStrikesIncrease.Add(_skillElementalStrikes.passData.damagePair.GetKey(0), _skillElementalStrikes.passData.damagePair.GetValue(0));
                            if (strikes % 4 == 1) _damElementStrikesIncrease.Add(_skillElementalStrikes.passData.damagePair.GetKey(1), _skillElementalStrikes.passData.damagePair.GetValue(1));
                            if (strikes % 4 == 2) _damElementStrikesIncrease.Add(_skillElementalStrikes.passData.damagePair.GetKey(2), _skillElementalStrikes.passData.damagePair.GetValue(2));
                            break;
                        case 3:
                            if (strikes % 4 == 0) _damElementStrikesIncrease.Add(_skillElementalStrikes.passData.damagePair.GetKey(0), _skillElementalStrikes.passData.damagePair.GetValue(0));
                            if (strikes % 4 == 1) _damElementStrikesIncrease.Add(_skillElementalStrikes.passData.damagePair.GetKey(1), _skillElementalStrikes.passData.damagePair.GetValue(1));
                            if (strikes % 4 == 2) _damElementStrikesIncrease.Add(_skillElementalStrikes.passData.damagePair.GetKey(2), _skillElementalStrikes.passData.damagePair.GetValue(2));
                            if (strikes % 4 == 3) _damElementStrikesIncrease.Add(_skillElementalStrikes.passData.damagePair.GetKey(3), _skillElementalStrikes.passData.damagePair.GetValue(3));
                            break;
                    }
                }
                break;
            case CombatEvent.Hit:
                explosiveHit();
                concussiveWave();
                void explosiveHit()
                {
                    if (_skillExplosiveHit is null) return;
                    if (Br.combat.counterHit % 3 != 0) return;
                    PassData container = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(new Element[2] { Element.Physical, Element.Fire }, 1f, _skillExplosiveHit.passData.damagePair)
                    };
                    SpellMain ex = SpellMain.Sp(_skillExplosiveHit.spell, Br.combat.MyTarget);
                     ex.areaOfEffect += _skillExplosiveHit.level + 1;
                    ex.InitializeMe(Br, container);
                }
                void concussiveWave()
                {
                    if (_skillConcussiveWave is null) return;
                    if (Random.value > _skillConcussiveWave.valueGeneric) return;
                    PassData container = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage()
                    };
                    SpellMain spell = SpellMain.Sp(_skillConcussiveWave.spell, Br);
                    spell.areaOfEffect += _skillConcussiveWave.level + 4;
                    spell.InitializeMe(Br, container);
                }
                break;
            case CombatEvent.Miss:
                spectralRicochet();
                void spectralRicochet()
                {
                    if (_skillSpectralRicochet) return;
                    if (Random.value > _skillSpectralRicochet.valueGeneric) return;
                    Vector3 dir = Utils.Direction(myShield.position, Br.combat.MyTarget == null ? myShield.position + Br.myTransform.forward : Br.combat.MyTarget.myTransform.position);
                    int ricochet = Br.character.GetStat(Stats.Ricochet);
                    PassData containerThrow = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Magic, 1f, _skillSpectralRicochet.passData.damagePair)
                    };
                    ricochet += (int)_skillSpectralRicochet.passData.stats[0].value;

                    SpellMain spell = SpellMain.Sp(_skillSpectralRicochet.spell, Utils.LevelV3(myShield.position), Quaternion.LookRotation(dir));
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
                    if (_skillSpikedRim is null) return;
                    if (!(otherBrain is not null && specialty == SpellMain.Specialty.Melee)) return;
                    float damageValue = Br.character.GetStat(Stats.ResistPhysical) * _skillSpikedRim.valueGeneric;
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
                statue();
                void statue()
                {
                    if (_skillStatue is null) return;
                    if (Random.value > 0.2f) return;
                    if (!_canStatueSpawn) return;
                    if (Br.loco.IsMoving) return;
                    SpellMain spell = SpellMain.Sp(_skillStatue.spell, Br.myTransform.position);
                    BuffStats buffStats = _skillStatue.passData.stats[Random.Range(0, _skillStatue.passData.stats.Length)];
                    PassData pd = new PassData()
                    {
                        hasStats = true,
                        stats = new BuffStats[1] { buffStats }
                    };
                    spell.InitializeMe(Br, pd);
                    _queueStatues.Enqueue(spell);
                    if (_queueStatues.Count <= _numMaxStatues) return;
                    SpellMain previous = _queueStatues.Dequeue();
                 //   _queueStatues.Dequeue();
                    previous.MyPhase = SpellMain.Phase.EndEnd;
                    StartCoroutine(cooldown());
                    return;
                    
                    IEnumerator cooldown()
                    {
                        _canStatueSpawn = false;
                        yield return Ga.me.wait50;
                        _canStatueSpawn = true;
                    }
                }

                break;
            case CombatEvent.Block:
                shockwaveBlock();
                void shockwaveBlock()
                {
                    if (_skillShockwaveBlock is null) return;
                    PassData passData = new PassData()
                    {
                        hasKnockback = true,
                        knockbackPower = Br.character.GetStat(Stats.KnockBack) + _skillShockwaveBlock.passData.knockbackPower,
                    };
                    StartCoroutine(delay());
                    return;
                    
                    IEnumerator delay()
                    {
                        yield return Ga.me.wait01;
                        SpellMain push = SpellMain.Sp(_skillShockwaveBlock.spell, Utils.LevelV3(myShield.position));
                        push.visual.SetSpawnHeight(myShield.position.y);
                        push.InitializeMe(Br, passData);
                    }
                }
                break;
            case CombatEvent.Kill:
                arcaneHarvest();
                funeralTornado();
                void arcaneHarvest()
                {
                    if (_skillArcaneHarvest is null) return;
                    if (otherBrain != null) return;
                    if (otherBrain.character.GetStat(Stats.DamMagic) < Br.character.GetStat(Stats.DamMagic)) return;
                    Br.character.CharacterInjectData(GenChange.Add, _skillArcaneHarvest.passData.stats[0]);
                }
                void funeralTornado()
                {
                    if (_skillFuneralTornado is null) return;
                    if (_cooldownFuneralTornado > 0f) return;
                    SpellMain spell = SpellMain.Sp(_skillFuneralTornado.spell, Br.myTransform.position);
                    spell.transporter.target = Br.myTransform;
                    PassData pd = new PassData()
                    {
                        hasDamage = true,
                        damagePair = new MyDuo<Element, float>(new Element[1] { Element.Physical }, new float[1]{ 2 * Br.character.GetDamage(Element.Physical).GetValue(0)})
                    };
                    spell.lifeTime = _skillFuneralTornado.valueGeneric;
                    spell.areaOfEffect += _skillFuneralTornado.level * 2;
                    spell.InitializeMe(Br, pd);
                    StartCoroutine(cooldown());
                    return;
                    IEnumerator cooldown()
                    {
                        _cooldownFuneralTornado = CONST_CooldownFuneralTornado;
                        while (_cooldownFuneralTornado > 0)
                        {
                            _cooldownFuneralTornado -= Time.deltaTime;
                            yield return null;
                        }
                    }
                }

                break;
            
            
                void advanceGuard(bool beginPhase)
                {
                    if (_skillAdvanceGuard is null) return;
                    if (otherBrain is null) return;
                    if (beginPhase)
                    {
                        if (!Br.loco.IsMoving) return;
                        if (Combat.IsFlanked(Br.myTransform, otherBrain.myTransform)) return;
                        _canAdvanceGuard = true;
                        _spellAdvanceGuard.visual.PlayDefault();
                        Br.character.CharacterInjectData(GenChange.Add, _skillAdvanceGuard.passData.stats[0]);
                        return;
                    }
                    if (!_canAdvanceGuard) return;
                    _canAdvanceGuard = false;
                    Br.character.CharacterInjectData(GenChange.Remove, _skillAdvanceGuard.passData.stats[0]);
                }

        }

    }
    

}

