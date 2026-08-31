using UnityEngine;
using UnityEngine.UI;

public class LevelUpCard : MonoBehaviour
{
    SoSkill _mySkill;

    public void InjectSkill(SoSkill skill)
    {
        _mySkill = skill;
    }
}
