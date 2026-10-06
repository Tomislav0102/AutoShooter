using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

[System.Serializable]
public class BuffStats
{
    public Stats stat;
    public BuffType buffType;
    public BuffData data;

    public BuffStats(Stats stat, BuffType buffType, float val, float dur = float.PositiveInfinity)
    {
        this.stat = stat;
        this.buffType = buffType;
        data = new BuffData(val, dur);
    }
}

[System.Serializable]
public class BuffEffects
{
    [ReadOnly] public Brain myBrain;
    public Status.Effect effect;
    public BuffData data;

    public BuffEffects() { }
    public BuffEffects(Brain brain, Status.Effect effect, float val, float dur = float.PositiveInfinity) 
    {
        myBrain = brain;
        this.effect = effect;
        data = new BuffData(val, dur);
    }
        
    public bool IsDot() => effect == Status.Effect.Bleeding_dot || 
                           effect == Status.Effect.Burning_dot || 
                           effect == Status.Effect.Chilled_dot || 
                           effect == Status.Effect.Jolted_dot ||  
                           effect == Status.Effect.Poisoned_dot;
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



