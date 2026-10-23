using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

[System.Serializable]
public class BuffStats: BuffData
{
    public Stats stat;
    public BuffType buffType;

  //  public BuffStats(Stats stat, BuffType buffType, float val, float dur = float.PositiveInfinity)
    public BuffStats(Stats stat, BuffType buffType, float val, float dur = float.PositiveInfinity) : base(val, dur)
    {
        this.stat = stat;
        this.buffType = buffType;
    }
}

[System.Serializable]
public class BuffEffects : BuffData
{
    public Status.Effect effect;
   // public BuffEffects(Brain brain, Status.Effect effect, float val, float dur = float.PositiveInfinity) 
    public BuffEffects(Status.Effect effect, float val, float dur = float.PositiveInfinity, Brain brain = null) : base(val, dur, brain)
    {
        this.effect = effect;
    }
        
    public bool IsDot() => effect == Status.Effect.Bleeding_dot || 
                           effect == Status.Effect.Burning_dot || 
                           effect == Status.Effect.Chilled_dot || 
                           effect == Status.Effect.Jolted_dot ||  
                           effect == Status.Effect.Poisoned_dot;
}

public class BuffData
{
    [ReadOnly] public Brain myBrain;
    public float value;
    public bool permanent = true; 
    [field:SerializeField] public float Duration { get; private set; }

    public BuffData(float val, float dur = float.PositiveInfinity, Brain brain = null)
    {
        value = val;
        Duration = dur;
        permanent = float.IsPositiveInfinity(Duration);
        myBrain = brain;
    }
}



