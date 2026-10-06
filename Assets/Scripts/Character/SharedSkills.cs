using System;
using System.Collections;
using UnityEngine;

public class SharedSkills : MonoBehaviour, IIniBrain
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            value.skills.onSkillIncrease += SkillIncreaseCallback;
        }
    }
    Brain _br;
    #region SKILL LAST STAND
    bool _lastStandCooldownTimerDone = true;
    bool _lastStandHealthAboveTreshold = true;
    #endregion


    void OnDisable()
    {
        Br.skills.onSkillIncrease -= SkillIncreaseCallback;
    }
    void SkillIncreaseCallback(SoSkill newSkill)
    {
        switch (newSkill.skillName)
        {
            case SkillName.VitalityBoost:
                SimpleStatSkillApply(SkillName.VitalityBoost);
                break;
            case SkillName.Adrenaline:
                SimpleStatSkillApply(SkillName.Adrenaline);
                break;
            case SkillName.EagleEye:
                SimpleStatSkillApply(SkillName.EagleEye);
                break;
            case SkillName.HeavyHitter:
                SimpleStatSkillApply(SkillName.HeavyHitter);
                break;
            case SkillName.Haste:
                SimpleStatSkillApply(SkillName.Haste);
                break;
            case SkillName.Scholar:
                SimpleStatSkillApply(SkillName.Scholar);
                break;
            case SkillName.Enlarge:
                SimpleStatSkillApply(SkillName.Enlarge);
                break;
            case SkillName.ExtraLife:
                Br.health.life++;
                break;
            case SkillName.ExtraSkillChoice:
                BuffStats buffStats = new BuffStats(Stats.ExtraSkillChoice, BuffType.Added, 1);
                Br.character.CharacterInjectData(GenChange.Add, buffStats);
                break;
            case SkillName.RotatingSwords:
                rotatingSwords();
                void rotatingSwords()
                {
                    SpellGroup groupSwords = SpellGroup.Gr(Ga.me.spells.groupOrbitalSwords, Br);
                    OrbitalGroup orbitalGroupSwords = groupSwords as OrbitalGroup;
                    orbitalGroupSwords.orbitingAnchor = Br.myTransform;

                    int numOfObjects = 2 * (newSkill.level + 1);
                    SpellMain[] swords = new SpellMain[numOfObjects];
                    swords[0] = Ga.me.spells.swordFire;
                    float multiplier = 1f / 3f;
                    multiplier = 1f;
                    PassData pdFire = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Fire, multiplier)
                    };
                    PassData pdIce = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Ice, multiplier)
                    };
                    PassData pdElectricity = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Electricity, multiplier)
                    };
                    PassData[] pds = new PassData[numOfObjects];
                    pds[0] = pdFire;
                    switch (numOfObjects)
                    {
                        case 2:
                            swords[1] = Ga.me.spells.swordFire;
                            pds[1] = pdFire;
                            Br.character.onStatChange += (Stats st) =>
                            {
                                if (st == Stats.DamFire)
                                {
                                    groupSwords.spellsRunning[0].pd.damagePair = Br.character.GetDamage(Element.Fire, multiplier);
                                    groupSwords.spellsRunning[1].pd.damagePair = Br.character.GetDamage(Element.Fire, multiplier);
                                }
                            };
                            break;
                        case 4:
                            swords[1] = Ga.me.spells.swordIce;
                            swords[2] = Ga.me.spells.swordFire;
                            swords[3] = Ga.me.spells.swordIce;
                            pds[1] = pdIce;
                            pds[2] = pdFire;
                            pds[3] = pdIce;
                            Br.character.onStatChange += (Stats st) =>
                            {
                                if (st == Stats.DamFire)
                                {
                                    groupSwords.spellsRunning[0].pd.damagePair = Br.character.GetDamage(Element.Fire, multiplier);
                                    groupSwords.spellsRunning[2].pd.damagePair = Br.character.GetDamage(Element.Fire, multiplier);
                                }
                                if (st == Stats.DamIce)
                                {
                                    groupSwords.spellsRunning[1].pd.damagePair = Br.character.GetDamage(Element.Ice, multiplier);
                                    groupSwords.spellsRunning[3].pd.damagePair = Br.character.GetDamage(Element.Ice, multiplier);
                                }
                            };
                            break;
                        case 6:
                            swords[1] = Ga.me.spells.swordIce;
                            swords[2] = Ga.me.spells.swordElectricity;
                            swords[3] = Ga.me.spells.swordFire;
                            swords[4] = Ga.me.spells.swordIce;
                            swords[5] = Ga.me.spells.swordElectricity;
                            pds[1] = pdIce;
                            pds[2] = pdElectricity;
                            pds[3] = pdFire;
                            pds[4] = pdIce;
                            pds[5] = pdElectricity;
                            Br.character.onStatChange += (Stats st) =>
                            {
                                if (st == Stats.DamFire)
                                {
                                    groupSwords.spellsRunning[0].pd.damagePair = Br.character.GetDamage(Element.Fire, multiplier);
                                    groupSwords.spellsRunning[3].pd.damagePair = Br.character.GetDamage(Element.Fire, multiplier);
                                }
                                if (st == Stats.DamIce)
                                {
                                    groupSwords.spellsRunning[1].pd.damagePair = Br.character.GetDamage(Element.Ice, multiplier);
                                    groupSwords.spellsRunning[4].pd.damagePair = Br.character.GetDamage(Element.Ice, multiplier);
                                }
                                if (st == Stats.DamElectricity)
                                {
                                    groupSwords.spellsRunning[2].pd.damagePair = Br.character.GetDamage(Element.Electricity, multiplier);
                                    groupSwords.spellsRunning[5].pd.damagePair = Br.character.GetDamage(Element.Electricity, multiplier);
                                }
                            };
                            break;
                        default:
                            print("should only be 2, 4, or 6 swords.");
                            return;
                    }
                    groupSwords.InitializeMe(Br, new MyDuo<SpellMain, PassData>(swords, pds));
                }
                break;
        }
    }


    void SimpleStatSkillApply(SkillName skillName)
    {
        if (!Br.skills.TryGetFromGroup(skillName, out SoSkill skill)) return;
        foreach (BuffStats stat in skill.passData.stats)
        {
            Br.character.CharacterInjectData(GenChange.Add, stat);
        }
    }

    public void CombatEventCallback(CombatEvent combatEvent, Brain otherBrain = null, SpellMain.Specialty specialty = SpellMain.Specialty.General)
    {
        switch (combatEvent)
        {
            case CombatEvent.Hit:
                riposte(true);
                break;
            case CombatEvent.Block:
            case CombatEvent.Dodge:
                riposte();
                break;
        }

        void riposte(bool onlyRemove = false)
        {
            if (!Br.skills.TryGetFromGroup(SkillName.Riposte, out SoSkill skill)) return;
            for (int i = 0; i < skill.passData.stats.Length; i++)
            {
                Br.character.CharacterInjectData(GenChange.Remove, skill.passData.stats[i]);
            }
            if (onlyRemove) return;
            for (int i = 0; i < skill.passData.stats.Length; i++)
            {
                Br.character.CharacterInjectData(GenChange.Add, skill.passData.stats[i]);
            }
        }
    }

    public void HealthMonitor(float currentHealthPercentage)
    {
        lastStand();
        return;
        
        void lastStand()
        {
            if (currentHealthPercentage > 0.9f)
            {
                _lastStandHealthAboveTreshold = true;
                return;
            }
            if (!Br.skills.TryGetFromGroup(SkillName.LastStand, out SoSkill skill)) return;
            if (!_lastStandCooldownTimerDone) return;
            if (!_lastStandHealthAboveTreshold) return;
            _lastStandCooldownTimerDone = false;
            _lastStandHealthAboveTreshold = false;
            Br.status.StatusInjectData(GenChange.Add, skill.passData.effects[0]);
            StartCoroutine(delay(skill.valueGeneric));
            return;

            IEnumerator delay(float time)
            {
                yield return new WaitForSeconds(time);
                _lastStandCooldownTimerDone = true;
            }
        }
    }
}
