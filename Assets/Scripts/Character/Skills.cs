using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;


public class Skills : MonoBehaviour, IIniBrain
{

    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            List<SoSkill> tempSkills = new List<SoSkill>();
            if (useShared) tempSkills.AddRange(Resources.LoadAll<SoSkill>("skills shared"));
            if (useKnight) tempSkills.AddRange(Resources.LoadAll<SoSkill>("skills knight"));
            if (useMage) tempSkills.AddRange(Resources.LoadAll<SoSkill>("skills mage"));
            if (useArcher) tempSkills.AddRange(Resources.LoadAll<SoSkill>("skills archer"));
            
            int enumLength = System.Enum.GetNames(typeof(SkillName)).Length;
            Dictionary<SkillName, List<SoSkill>> dict = new Dictionary<SkillName, List<SoSkill>>();
            for (int i = 0; i < enumLength; i++)
            {
                dict.Add((SkillName)i, allSkillsByName((SkillName)i));
            } 
            List<Group> tempGroups = new List<Group>();
            foreach (KeyValuePair<SkillName, List<SoSkill>> item in dict)
            {
                if (item.Value.Count == 0) continue;
                tempGroups.Add(new Group(item.Value.ToArray()));
            }
            _group = tempGroups.ToArray();
            if (useKnight)
            {
                OvrLevelSpecificSkill(SkillName.KnightBase, 0);
                OvrLevelSpecificSkill(SkillName.GrandCrescendo, 1);
            }
            if (useMage) OvrLevelSpecificSkill(SkillName.MagicMissile, 0);
            if (useArcher) OvrLevelSpecificSkill(SkillName.ArcherBase, 0);
            return;

            List<SoSkill> allSkillsByName(SkillName skillName)
            {
                List<SoSkill> temp = new List<SoSkill>();
                foreach (SoSkill s in tempSkills)
                {
                    if (s.skillName == skillName) temp.Add(s);
                }
                return temp;
            }

        }
    }
    Brain _br;

    [SerializeField] SoSkill[] replacements;
    public SoSkill myBasic;
    [SerializeField] bool useShared, useKnight, useMage, useArcher;
    [ShowInInspector, ReadOnly] Group[] _group;
    [SerializeField] UnityEvent<SoSkill> skillIncreaseEv;
    #region DEBUG
    
    [Title("Debug")]    
    public SoSkill[] skillsToLevel;
    [Button]
    public void LevelAboveSkills()
    {
        for (int i = 0; i < skillsToLevel.Length; i++)
        {
            if (skillsToLevel == null) continue;
            OvrLevelSpecificSkill(skillsToLevel[i].skillName, 0);
        }
    }
    [Button]
    public void LevelUpNormal()
    {
        HashSet<SoSkill> skills = new HashSet<SoSkill>();

        foreach (Group g in _group)
        {
            if (!g.canAcquire) continue;
            if (g.CanLevel(out SoSkill nextLevelSkill)) skills.Add(nextLevelSkill);
        }

        List<SoSkill> tempSkills = new List<SoSkill>();
        foreach (SoSkill s in skills)
        {
            tempSkills.Add(s);
        }
        tempSkills = Utils.RandomListByType(tempSkills);
        SoSkill[] chosenSkills = new SoSkill[3];
        for (int i = 0; i < 3; i++)
        {
            if (tempSkills.Count > i)
            {
                chosenSkills[i] = tempSkills[i];
                continue;
            }
            chosenSkills[i] = replacements[Random.Range(0, replacements.Length)];
        }
        Ga.me.InjectSkills(chosenSkills);
    }
    #endregion

    
    void OvrLevelSpecificSkill(SkillName skillName, int level)
    {
        foreach (Group g in _group)
        {
            if (g.skillName != skillName) continue;
            g.levelCurrent = level;
            return;
        }
    }
    public void SkillIncrease(SkillName skillName)
    {
        if (skillName == SkillName.ReplacementGold)
        {
            skillIncreaseEv.Invoke(replacements[0]);
            return;
        }
        if (skillName == SkillName.ReplacementHeal)
        {
            skillIncreaseEv.Invoke(replacements[1]);
            return;
        }
        foreach (Group g in _group)
        {
            if (g.skillName != skillName) continue;
            if (!g.CanLevel(out _)) return;
            g.levelCurrent++;
            skillIncreaseEv.Invoke(g.MySkill());
            break;
        }
    }


    [System.Serializable]
    public class Group
    {
        public SkillName skillName;
        public bool canAcquire; //true by default, some skills have requirements (e.g. after finishing the game once)
        public int levelCurrent;
        SoSkill[] _mySkills;

        public Group(SoSkill[] skills)
        {
            canAcquire = true;
            levelCurrent = -1;
            _mySkills = skills;
            skillName = _mySkills[0].skillName;
        }
        public SoSkill MySkill()
        {
            return levelCurrent < 0 ? null : _mySkills[levelCurrent];
        }
        public bool CanLevel(out SoSkill nextLevelSkill) 
        {
            if (levelCurrent + 1 < _mySkills.Length)
            {
                nextLevelSkill = _mySkills[levelCurrent + 1];
                return true;
            }
            nextLevelSkill = null;
            print("Can't level " + skillName + ". Maxed out." );
            return false;
        }
    }

    public bool TryGetFromGroup(SkillName skillName, out SoSkill skill)
    {
        foreach (Group item in _group)
        {
            if (item.skillName != skillName) continue;
            if (!item.canAcquire) continue;
            if (item.levelCurrent < 0) continue;
            skill = item.MySkill();
            return true;
        }
        skill = null;
        return false;
    }
    SoSkill[] CurrentSkills()
    {
        List<SoSkill> sk = new List<SoSkill>();
        foreach (Group item in _group)
        {
            if (!item.canAcquire) continue;
            if (item.levelCurrent < 0) continue;
            sk.Add(item.MySkill());
        }
        return sk.ToArray();
    }




}


