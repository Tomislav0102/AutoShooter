using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpCardUi : MonoBehaviour
{
    SoSkill _mySkill;
    [SerializeField] TextMeshProUGUI myText;

    public void InjectSkill(SoSkill skill)
    {
        _mySkill = skill;
        myText.text = $"{_mySkill.skillName.ToString()}";
    }

    public void ButtonClick()
    {
        if (_mySkill is null) return;
        Ga.me.team.playersBrain.skills.SkillIncrease(_mySkill.skillName);
        Ga.me.uiManager.OpenPanel(PanelType.Game);
    }

}
