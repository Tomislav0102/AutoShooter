using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class SoCharacter : SerializedScriptableObject
{
    public Dictionary<Stats, int> stats = new Dictionary<Stats, int>();
    [Button]
    void ClickMe()
    {
    }
}
