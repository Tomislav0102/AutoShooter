using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


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
            _allSkills =  tempSkills.ToArray();
            
            int enumLength = System.Enum.GetNames(typeof(SkillName)).Length;
            Dictionary<SkillName, List<SoSkill>> dict = new Dictionary<SkillName, List<SoSkill>>();
            for (int i = 0; i < enumLength; i++)
            {
                dict.Add((SkillName)i, AllSkillsByName((SkillName)i));
            } 
            List<Group> tempGroups = new List<Group>();
            foreach (KeyValuePair<SkillName, List<SoSkill>> item in dict)
            {
                if (item.Value.Count == 0) continue;
                tempGroups.Add(new Group(item.Value.ToArray()));
            }
            tempGroups.Add(new Group(new SoSkill[1]{baseSkill}, 0));
            _group = tempGroups.ToArray();
        }
    }
    Brain _br;
    public MyDuo<bool, SoSkill> pair;

    public SoSkill baseSkill;
    [SerializeField] SoSkill[] replacements;
    [SerializeField] bool useShared, useKnight, useMage, useArcher;
    SoSkill[] _allSkills;
    [ShowInInspector, ReadOnly] Group[] _group;
    

    [Button]
    public void LevelUp()
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
    public void SkillIncrease(SkillName skillName)
    {
        if (skillName == SkillName.ReplacementGold)
        {

            return;
        }
        if (skillName == SkillName.ReplacementHeal)
        {

            return;
        }
        foreach (Group g in _group)
        {
            if (g.skillName != skillName) continue;
            g.levelCurrent++;
            break;
        }
    }


    [System.Serializable]
    public class Group
    {
        public SkillName skillName;
        public bool canAcquire = true; //true by default, some skills have requirements (e.g. after finishing the game once)
        public int levelCurrent = -1;
        SoSkill[] _mySkills;

        public Group(SoSkill[] skills, int startLevel = -1)
        {
            canAcquire = true;
            levelCurrent = startLevel;
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

    List<SoSkill> AllSkillsByName(SkillName skillName)
    {
        List<SoSkill> tempSkills = new List<SoSkill>();
        foreach (SoSkill s in _allSkills)
        {
            if (s.skillName == skillName) tempSkills.Add(s);
        }
        return tempSkills;
    }



}


