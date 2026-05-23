using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class SoCharacter : SerializedScriptableObject
{
    public Dictionary<Stats, int> stats = new Dictionary<Stats, int>();
    public int level;
    public Dictionary<SkillReq, int> skillRequirements = new Dictionary<SkillReq, int>();
    [Button]
    void ResetSkillReqs()
    {
        skillRequirements = new Dictionary<SkillReq, int>();
        for (int i = 0; i < System.Enum.GetNames(typeof(SkillReq)).Length; i++)
        {
            skillRequirements.Add((SkillReq)i, 0);
        }
        skillRequirements[SkillReq.Block] = 1;
    }
    [Title("Debug")]
    public int someInt;
    [Button]
    void ClickMe()
    {
        stats = new Dictionary<Stats, int>();
        for (int i = 0; i < System.Enum.GetNames(typeof(Stats)).Length; i++)
        {
            stats.Add((Stats)i, someInt);
        }
    }
}
