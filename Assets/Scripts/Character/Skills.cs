using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class Skills : MonoBehaviour, IIniBrain
{
    [HideInInspector] public System.Action<SoSkill> onSkillIncrease;
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            if (!useShared && !useKnight && !useMage && !useArcher) return; //enemy

            List<SoSkill> tempSkills = new List<SoSkill>();
            if (useShared) tempSkills.AddRange(Resources.LoadAll<SoSkill>("skills shared"));
            if (useKnight) tempSkills.AddRange(Resources.LoadAll<SoSkill>("skills knight"));
            if (useMage) tempSkills.AddRange(Resources.LoadAll<SoSkill>("skills mage"));
            if (useArcher) tempSkills.AddRange(Resources.LoadAll<SoSkill>("skills archer"));
            tempSkills.Add(myBasic);

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
            OvrLevelSpecificSkill(myBasic.skillName);

            //debugs
            if (useKnight)
            {
                OvrLevelSpecificSkill(SkillName.RotatingSwords);
                
            }
            if (useMage)
            {
                OvrLevelSpecificSkill(SkillName.DragonsBreath);

            }
            if (useArcher)
            {

            }
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
    public SoSkill myBasic, myUltimate;

    [SerializeField] bool useShared, useKnight, useMage, useArcher;
    Group[] _group;
    UiLevelUp _uiLevelUp;
    
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
    public void SelectNewPossibleSkills()
    {
        HashSet<SoSkill> skills = new HashSet<SoSkill>();

        foreach (Group g in _group)
        {
            if (!g.canAcquire) continue;
            if (myUltimate is not null &&
                g.MySkill().skillType == SkillType.Ultimate && 
                g.skillName != myUltimate.skillName) continue;
            if (g.CanLevel(out SoSkill nextLevelSkill)) skills.Add(nextLevelSkill);
        }

        List<SoSkill> tempSkills = new List<SoSkill>();
        foreach (SoSkill s in skills)
        {
            tempSkills.Add(s);
        }
        tempSkills = Utils.RandomListByType(tempSkills);
        SoSkill[] final = new SoSkill[3 + Br.character.GetStat(Stats.ExtraSkillChoice)];
        for (int i = 0; i < final.Length; i++)
        {
            if (tempSkills.Count > i)
            {
                final[i] = tempSkills[i];
                continue;
            }
            final[i] = replacements[Random.Range(0, replacements.Length)];
        }
        _uiLevelUp.InjectSkills(final);
    }
    #endregion

    void Start()
    {
        _uiLevelUp = Ga.me.uiManager.PanelByType(PanelType.LevelUp).GetComponent<UiLevelUp>();
    }

    void OvrLevelSpecificSkill(SkillName skillName, int level = 0)
    {
        foreach (Group g in _group)
        {
            if (g.skillName != skillName) continue;
            g.levelCurrent = level;
            StartCoroutine(delay(g)); //debug
            return;
        }
        return;
        IEnumerator delay(Group g)
        {
            yield return null;
            onSkillIncrease?.Invoke(g.MySkill());
        }
    }
    public void SkillIncrease(SkillName skillName)
    {
        if (skillName == SkillName.ReplacementGold)
        {
            onSkillIncrease?.Invoke(replacements[0]);
            return;
        }
        if (skillName == SkillName.ReplacementHeal)
        {
            onSkillIncrease?.Invoke(replacements[1]);
            return;
        }
        foreach (Group g in _group)
        {
            if (g.skillName != skillName) continue;
            if (!g.CanLevel(out _)) return;
            g.levelCurrent++;
            onSkillIncrease?.Invoke(g.MySkill());
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
            return levelCurrent < 0 ? _mySkills[0] : _mySkills[levelCurrent];
        }
        public bool CanLevel(out SoSkill nextLevelSkill) 
        {
            if (levelCurrent + 1 < _mySkills.Length)
            {
                nextLevelSkill = _mySkills[levelCurrent + 1];
                return true;
            }
            nextLevelSkill = null;
           // print("Can't level " + skillName + ". Maxed out." );
            return false;
        }
    }

    public bool TryGetFromGroup(SkillName skillName, out SoSkill skill)
    {
        skill = null;
        foreach (Group item in _group)
        {
            if (item.skillName != skillName) continue;
            if (!item.canAcquire) return false;
            if (item.levelCurrent < 0) return false;
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


