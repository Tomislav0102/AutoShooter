using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;



// [System.Serializable]
// public class StatsGroup
// {
//     public GenChange change;
//     bool ShowBuffType() => change == GenChange.Add;
//     [ShowIf(nameof(ShowBuffType))] public BuffType buffType;
//     [FormerlySerializedAs("buff")] public BuffStats buffStats;
//         
//     public StatsGroup(GenChange change, BuffType buffType, BuffStats buffStats) 
//     {
//         this.change = change;
//         this.buffType = buffType;
//         this.buffStats = buffStats;
//     }
//     public StatsGroup(GenChange change, BuffStats buffStats) //for removal
//     {
//         this.change = change;
//         this.buffStats = buffStats;
//     }
// }

[System.Serializable]
public class BuffStats
{
    public Stats stat;
    public BuffType buffType;
    public BuffData data;

    public BuffStats(Stats stat, BuffType buffType, float val, float dur = float.PositiveInfinity)
    {
        this.stat = stat;
        data = new BuffData(val, dur);
    }
}

[System.Serializable]
public class BuffEffects
{
    [ReadOnly] public Brain brain;
    public Status.Effect effect;
    public BuffData data;

    public BuffEffects(Status.Effect effect, float val, float dur = float.PositiveInfinity) 
    {
        this.effect = effect;
        data = new BuffData(val, dur);
    }
        
    public bool IsDot() => effect == Status.Effect.Bleeding || 
                           effect == Status.Effect.Burning || 
                           effect == Status.Effect.Freezing || 
                           effect == Status.Effect.Jolted ||  
                           effect == Status.Effect.Poisoned;
}

[System.Serializable]
public struct BuffData
{
    public float value;
    public bool permanent; 
    [field:SerializeField] public float Duration { get; private set; }

    public BuffData(float val, float dur = float.PositiveInfinity)
    {
        value = val;
        Duration = dur;
        permanent = float.IsPositiveInfinity(Duration);
    }
}



