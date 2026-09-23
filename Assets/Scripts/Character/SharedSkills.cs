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
    

    public void SkillIncreaseCallback(SoSkill newSkill)
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
        }
    }


    void SimpleStatSkillApply(SkillName skillName)
    {
        if (!Br.skills.TryGetFromGroup(skillName, out SoSkill skill)) return;
        foreach (StatsGroup stat in skill.stats)
        {
            Br.character.CharacterInjectData(stat);
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
            StatsGroup rem = skill.stats[0];
            rem.change = GenChange.Remove;
            Br.character.CharacterInjectData(rem);
            if (onlyRemove) return;
            
            Br.character.CharacterInjectData(skill.stats[0]);
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
            StartCoroutine(delay(skill.floatGeneric));
            return;

            IEnumerator delay(float time)
            {
                yield return new WaitForSeconds(time);
                _lastStandCooldownTimerDone = true;
            }
        }
    }
}
