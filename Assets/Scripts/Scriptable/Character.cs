using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class Character : SerializedScriptableObject
{
    public string myName;
    public Dictionary<StatsPrimary, int> primary;
    public Dictionary<StatsSecondary, int> secondary;

    [Button]
    void ResetStats()
    {
        primary = new Dictionary<StatsPrimary, int>();
        secondary = new Dictionary<StatsSecondary, int>();
        for (int i = 0; i < System.Enum.GetNames(typeof(StatsPrimary)).Length; i++)
        {
            primary.Add((StatsPrimary)i, 0);
        }
        for (int i = 0; i < System.Enum.GetNames(typeof(StatsSecondary)).Length; i++)
        {
            secondary.Add((StatsSecondary)i, 0);
        }
    }
    
}
