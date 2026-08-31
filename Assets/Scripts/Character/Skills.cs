using System;
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
        }
    }
    Brain _br;
    public MyDuo<bool, SoSkill> pair;

    [SerializeField] bool useShared, useKnight, useMage, useArcher;
    [ShowInInspector, ReadOnly] SoSkill[] _allSkills;
    [ShowInInspector, ReadOnly] Group[] _group;
    

    [Button]
    public void LevelUp()
    {
        List<SoSkill> skills = new List<SoSkill>();

        if (skills.Count == 0)
        {
            print("no skills left to choose, fix needed!");
            return;
        }
        
        if (skills.Count < 3) print("less than 3 skills remain, need to fix UI!");
        
        skills = Utils.RandomListByType(skills);
        SoSkill[] chosenSkills = new SoSkill[3];
        for (int i = 0; i < chosenSkills.Length; i++)
        {
            chosenSkills[i] = skills[i];
        }
        Ga.me.InjectSkills(chosenSkills);
    }
    

    [System.Serializable]
    public class Group
    {
        public bool canAcquire = true; //true by default, some skills have requirements (e.g. after finishing the game once)
        public int levelCurrent = -1;
        public SkillName skillName;

        public Group(SoSkill skill)
        {
            canAcquire = true;
            levelCurrent = -1;
            skillName = skill.skillName;
        }
    }

    public bool TryGetFromNewGroup(SkillName skillName, out SoSkill skill)
    {
        skill = null;
        foreach (SoSkill s in _allSkills)
        {
            if (s.skillName != skillName) continue;
        }
        return false;
    }



}


