using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class SoCharacter : SerializedScriptableObject
{
    public Dictionary<Stats, int> baseStats = new Dictionary<Stats, int>();
   
    [Button]
    void SetAllStats(int value = 100)
    {
        baseStats = new Dictionary<Stats, int>();
        int count = System.Enum.GetNames(typeof(Stats)).Length;
        for (int i = 0; i < count; i++)
        {
            baseStats.Add((Stats)i, value);
        }
    }
    [Button]
    void SetAllStatsDetailed(int value = 100)
    {
        SetAllStats(value);
        baseStats[Stats.MeleeDamage] = 1;
        baseStats[Stats.RangedDamage] = 2;
        baseStats[Stats.MagicDamage] = 3;
        baseStats[Stats.Block] = 0;
        baseStats[Stats.KnockBack] = 0;
        baseStats[Stats.Projectiles] = 1;
        baseStats[Stats.Bounce] = 0;
        baseStats[Stats.Bounce] = 0;
        baseStats[Stats.Piercing] = 0;
        baseStats[Stats.Ricochet] = 0;
    }
}
