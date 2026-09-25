using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCardUi : MonoBehaviour
{
    public SoSkill MySkill
    {
        get  => _mySkill;
        set
        {
            _mySkill = value;
            if (_mySkill is not null)
            {
                infoText.text = value.skillName.ToString();
            }
        }
    }
    SoSkill _mySkill;
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI infoText;
}
