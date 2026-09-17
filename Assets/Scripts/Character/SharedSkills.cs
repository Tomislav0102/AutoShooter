using System;
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
    bool _canHaveLastStand = true;
    

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
            Br.character.BuffInjectData(stat);
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
            Br.character.BuffInjectData(rem);
            if (onlyRemove) return;
            
            Br.character.BuffInjectData(skill.stats[0]);
        }
    }
}
