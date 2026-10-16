using System;
using System.Collections;
using System.Collections.Generic;
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
    SpellGroup _groupRotSwords;
    SoSkill _skillRiposte;
    SoSkill _skillLastStand;
    bool _lastStandCooldownTimerDone = true;
    bool _lastStandHealthAboveTreshold = true;


    void OnDisable()
    {
        Br.skills.onSkillIncrease -= SkillIncreaseCallback;
    }
    void SkillIncreaseCallback(SoSkill newSkill)
    {
        switch (newSkill.skillId)
        {
            case SkillId.VitalityBoost:
            case SkillId.Adrenaline:
            case SkillId.EagleEye:
            case SkillId.HeavyHitter:
            case SkillId.Haste:
            case SkillId.Scholar:
                SimpleStatSkillApply(newSkill);
                break;
            case SkillId.ExtraLife:
                Br.health.life++;
                break;
            case SkillId.ExtraSkillChoice:
                BuffStats buffStats = new BuffStats(Stats.ExtraSkillChoice, BuffType.Added, 1);
                Br.character.CharacterInjectData(GenChange.Add, buffStats);
                break;
            case SkillId.RotatingSwords:
                rotatingSwords();
                void rotatingSwords()
                {
                    if (_groupRotSwords != null) Destroy(_groupRotSwords);
                    _groupRotSwords = SpellGroup.Gr(Ga.me.spells.groupOrbitalSwords, Br);
                    OrbitalGroup orbitalGroupSwords = _groupRotSwords as OrbitalGroup;
                    orbitalGroupSwords.orbitingAnchor = Br.myTransform;

                    int numOfObjects = 2 * (newSkill.level + 1);
                    SpellMain[] swords = new SpellMain[numOfObjects];
                    swords[0] = Ga.me.spells.swordFire;
                    PassData pdFire = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Fire)
                    };
                    PassData pdIce = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Ice)
                    };
                    PassData pdElectricity = new PassData()
                    {
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Electricity)
                    };
                    PassData[] pds = new PassData[numOfObjects];
                    pds[0] = pdFire;
                    switch (numOfObjects)
                    {
                        case 2:
                            swords[1] = Ga.me.spells.swordFire;
                            pds[1] = pdFire;
                            Br.character.onStatChange += (Stats st, float val) =>
                            {
                                if (st == Stats.DamFire)
                                {
                                    _groupRotSwords.spellsRunning[0].pd.damagePair = Br.character.GetDamage(Element.Fire);
                                    _groupRotSwords.spellsRunning[1].pd.damagePair = Br.character.GetDamage(Element.Fire);
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
                            Br.character.onStatChange += (Stats st, float val) =>
                            {
                                if (st == Stats.DamFire)
                                {
                                    _groupRotSwords.spellsRunning[0].pd.damagePair = Br.character.GetDamage(Element.Fire);
                                    _groupRotSwords.spellsRunning[2].pd.damagePair = Br.character.GetDamage(Element.Fire);
                                }
                                if (st == Stats.DamIce)
                                {
                                    _groupRotSwords.spellsRunning[1].pd.damagePair = Br.character.GetDamage(Element.Ice);
                                    _groupRotSwords.spellsRunning[3].pd.damagePair = Br.character.GetDamage(Element.Ice);
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
                            Br.character.onStatChange += (Stats st, float val) =>
                            {
                                if (st == Stats.DamFire)
                                {
                                    _groupRotSwords.spellsRunning[0].pd.damagePair = Br.character.GetDamage(Element.Fire);
                                    _groupRotSwords.spellsRunning[3].pd.damagePair = Br.character.GetDamage(Element.Fire);
                                }
                                if (st == Stats.DamIce)
                                {
                                    _groupRotSwords.spellsRunning[1].pd.damagePair = Br.character.GetDamage(Element.Ice);
                                    _groupRotSwords.spellsRunning[4].pd.damagePair = Br.character.GetDamage(Element.Ice);
                                }
                                if (st == Stats.DamElectricity)
                                {
                                    _groupRotSwords.spellsRunning[2].pd.damagePair = Br.character.GetDamage(Element.Electricity);
                                    _groupRotSwords.spellsRunning[5].pd.damagePair = Br.character.GetDamage(Element.Electricity);
                                }
                            };
                            break;
                        default:
                            return;
                    }
                    _groupRotSwords.mainPrefabPair = new MyDuo<SpellMain, PassData>(swords, pds);
                    _groupRotSwords.InitializeMe(Br);
                }
                break;
            case SkillId.Riposte:
                _skillRiposte = newSkill;
                break;
            case SkillId.LastStand:
                _skillLastStand = newSkill;
                break;
        }
    }


    void SimpleStatSkillApply(SoSkill skill)
    {
        if (skill is null) return;
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
            if (_skillRiposte is null) return;
            for (int i = 0; i < _skillRiposte.passData.stats.Length; i++)
            {
                Br.character.CharacterInjectData(GenChange.Remove, _skillRiposte.passData.stats[i]);
            }
            if (onlyRemove) return;
            for (int i = 0; i < _skillRiposte.passData.stats.Length; i++)
            {
                Br.character.CharacterInjectData(GenChange.Add, _skillRiposte.passData.stats[i]);
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
            if (_skillLastStand is null) return;
            if (!_lastStandCooldownTimerDone) return;
            if (!_lastStandHealthAboveTreshold) return;
            _lastStandCooldownTimerDone = false;
            _lastStandHealthAboveTreshold = false;
            Br.status.StatusInjectData(GenChange.Add, _skillLastStand.passData.effects[0]);
            StartCoroutine(delay(_skillLastStand.valueGeneric));
            return;

            IEnumerator delay(float time)
            {
                yield return new WaitForSeconds(time);
                _lastStandCooldownTimerDone = true;
            }
        }
        // void lastStand()
        // {
        //     if (currentHealthPercentage > 0.9f)
        //     {
        //         _lastStandHealthAboveTreshold = true;
        //         return;
        //     }
        //     if (!Br.skills.TryGetFromGroup(SkillName.LastStand, out SoSkill skill)) return;
        //     if (!_lastStandCooldownTimerDone) return;
        //     if (!_lastStandHealthAboveTreshold) return;
        //     _lastStandCooldownTimerDone = false;
        //     _lastStandHealthAboveTreshold = false;
        //     Br.status.StatusInjectData(GenChange.Add, skill.passData.effects[0]);
        //     StartCoroutine(delay(skill.valueGeneric));
        //     return;
        //
        //     IEnumerator delay(float time)
        //     {
        //         yield return new WaitForSeconds(time);
        //         _lastStandCooldownTimerDone = true;
        //     }
        // }
    }
}
