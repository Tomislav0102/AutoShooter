using System;
using UnityEngine;
using UnityEngine.Serialization;

public class UiLevelUp : MonoBehaviour
{
    [SerializeField] LevelUpCardUi[] levelUpCards;


    public void InjectSkills(SoSkill[] skills)
    {
        Ga.me.uiManager.OpenPanel(PanelType.LevelUp);
        int skillLength = skills.Length;
        for (int i = 0; i < levelUpCards.Length; i++)
        {
            if (i < skillLength)
            {
                levelUpCards[i].InjectSkill(skills[i]);
                levelUpCards[i].gameObject.SetActive(true);
            }
            else levelUpCards[i].gameObject.SetActive(false);
        }
    }
}
