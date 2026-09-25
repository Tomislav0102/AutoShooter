using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpCard : MonoBehaviour
{
    SoSkill _mySkill;
    [SerializeField] TextMeshProUGUI myText;
    Skills _playerSkill;

    void Start()
    {
        _playerSkill = Ga.me.team.playerTransform.GetComponent<Brain>().skills;
    }
    public void InjectSkill(SoSkill skill)
    {
        _mySkill = skill;
        myText.text = $"{_mySkill.skillName.ToString()}";
    }

    public void ButtonClick()
    {
        if (_mySkill is null) return;
        _playerSkill.SkillIncrease(_mySkill.skillName);
        Ga.me.uiManager.OpenPanel(PanelType.Game);
    }

}
