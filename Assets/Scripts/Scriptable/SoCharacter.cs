using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class SoCharacter : SerializedScriptableObject
{
    public Dictionary<Stats, int> stats = new Dictionary<Stats, int>();
   
    [Button]
    void SetAllStats(int value = 100)
    {
        stats = new Dictionary<Stats, int>();
        int count = System.Enum.GetNames(typeof(Stats)).Length;
        for (int i = 0; i < count; i++)
        {
            stats.Add((Stats)i, value);
        }
        
    }
}
