using System;
using System.Collections.Generic;
using UnityEngine;

public class UiSkill : MonoBehaviour
{
    [SerializeField] Transform parCards;
    [SerializeField] SkillCardUi prefabCard;
    List<SkillCardUi> _cards = new List<SkillCardUi>();


    void Start()
    {
       Ga.me.team.playersBrain.skills.onSkillIncrease += AddSkill;
    }
    void OnDestroy()
    {
        Ga.me.team.playersBrain.skills.onSkillIncrease -= AddSkill;
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
