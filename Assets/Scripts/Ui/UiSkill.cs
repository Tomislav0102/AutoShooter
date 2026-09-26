using System;
using System.Collections.Generic;
using UnityEngine;

public class UiSkill : MonoBehaviour
{
    [SerializeField] Transform parCards;
    [SerializeField] SkillCardUi prefabCard;
    List<SkillCardUi> _cards = new List<SkillCardUi>();


    void OnEnable()
    {
        Skills.OnSkillIncrease += AddSkill;
    }
    void OnDisable()
    {
        Skills.OnSkillIncrease -= AddSkill;
    }

    void AddSkill(SoSkill skill)
    {
        if (skill.skillType == SkillType.Replacement) return;
        
        foreach (SkillCardUi card in _cards)
        {
            if (card.MySkill.skillName == skill.skillName) //level up existing skill
            {
                card.MySkill = skill;
                return;
            }
        }
        SkillCardUi skillCard = Instantiate(prefabCard, parCards).GetComponent<SkillCardUi>();
        skillCard.MySkill = skill;
        _cards.Add(skillCard);
    }
}
