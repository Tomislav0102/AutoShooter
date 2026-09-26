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
        }
    }
    Brain _br;
    #region SKILL LAST STAND
    bool _lastStandCooldownTimerDone = true;
    bool _lastStandHealthAboveTreshold = true;
    #endregion


    void OnEnable()
    {
        Skills.OnSkillIncrease += SkillIncreaseCallback;
    }
    void OnDisable()
    {
        Skills.OnSkillIncrease -= SkillIncreaseCallback;
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
                BuffStats buffStats = new BuffStats(Stats.ExtraSkillChoice, BuffType.Added,1);
                Br.character.CharacterInjectData(GenChange.Add, buffStats);
                break;
        }
    }


    void SimpleStatSkillApply(SkillName skillName)
    {
        if (!Br.skills.TryGetFromGroup(skillName, out SoSkill skill)) return;
        foreach (BuffStats stat in skill.stats)
        {
            Br.character.CharacterInjectData(GenChange.Add, stat);
        }
    }

    public void CombatEventCallback(CombatEvent combatEvent, Brain otherBrain = null)
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
            Br.character.CharacterInjectData(GenChange.Remove, skill.stats[0]);
            if (onlyRemove) return;
            
            Br.character.CharacterInjectData(GenChange.Add, skill.stats[0]);
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
            Br.status.StatusInjectData(GenChange.Add, skill.buffEffect[0]);
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
