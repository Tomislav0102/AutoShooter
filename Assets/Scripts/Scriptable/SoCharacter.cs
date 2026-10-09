using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class SoCharacter : SerializedScriptableObject
{
   
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
        baseStats[Stats.DamPhysical] = 1;
        baseStats[Stats.DamFire] = 2;
        baseStats[Stats.DamIce] = 3;
        baseStats[Stats.DamElectricity] = 4;
        baseStats[Stats.DamPoison] = 5;
        baseStats[Stats.DamMagic] = 6;
        baseStats[Stats.ResistPhysical] = 0;
        baseStats[Stats.ResistFire] = 0;
        baseStats[Stats.ResistIce] = 0;
        baseStats[Stats.ResistElectricity] = 0;
        baseStats[Stats.ResistPoison] = 0;
        baseStats[Stats.ResistMagic] = 0;
        baseStats[Stats.Block] = 0;
        baseStats[Stats.Dodge] = 0;
        baseStats[Stats.Health] = 1000;
        baseStats[Stats.RegenerationRate] = 0;
        baseStats[Stats.KnockBack] = 0;
        baseStats[Stats.Projectiles] = 1;
        baseStats[Stats.Bounce] = 0;
        baseStats[Stats.Bounce] = 0;
        baseStats[Stats.Piercing] = 0;
        baseStats[Stats.Ricochet] = 0;
        baseStats[Stats.Size] = 0;
        baseStats[Stats.Duration] = 0;
        baseStats[Stats.ReflexSpells] = 0;
        baseStats[Stats.ReflectMelee] = 0;
        baseStats[Stats.ReflectProjectiles] = 0;
        baseStats[Stats.ExtraSkillChoice] = 0;
    }
    
    public Dictionary<Stats, int> baseStats = new Dictionary<Stats, int>();
}
