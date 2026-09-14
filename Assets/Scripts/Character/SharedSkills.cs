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
        Br.character.BuffInjectData(skill.stats);
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
            PassDataStats pdsRemove = new PassDataStats()
            {
                group = new PassDataStats.Group[1]
                {
                    new PassDataStats.Group(GenChange.Remove, skill.stats.group[0].buff)
                }
            };
            Br.character.BuffInjectData(pdsRemove);
            if (onlyRemove) return;
            Br.character.BuffInjectData(skill.stats);

        }
    }
}
