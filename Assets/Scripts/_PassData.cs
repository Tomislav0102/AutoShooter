using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;


[System.Serializable]
public class PassDataContainer
{
    public Brain myBrain;
    public bool canBeBlocked;
    public PassData[] data;

    
    // public PassDataContainer(Brain brain, PassData[] data, bool canBeBlocked = false)
    // {
    //     myBrain = brain;
    //     this.data = data;
    //     System.Array.Sort(data, (x, y) => x.priority.CompareTo(y.priority));
    //     this.canBeBlocked = canBeBlocked;
    // }
}

[System.Serializable]
public class PassData
{
    public enum HitEffectOnSpell { None, Nullify, Reflect }
    [ReadOnly] public int priority;
}


[System.Serializable]
public class PassDataManaShield : PassData
{
    public int manaShieldPoints;

    public PassDataManaShield(int manaShieldPoints)
    {
        priority = 20;
        this.manaShieldPoints = manaShieldPoints;
    }
}

[System.Serializable]
public class PassDataKnockBack : PassData
{
    [FormerlySerializedAs("knockBack")] public int knockBackPower;
    public Vector2 knockBackDirection; //ignored if = Vector2.zero

    public PassDataKnockBack(int knockBackPower, Vector2 knockBackDirection = new Vector2())
    {
        priority = 30;
        this.knockBackPower = knockBackPower;
        this.knockBackDirection = knockBackDirection;
    }
}
[System.Serializable]
public class PassDataMagnet : PassData
{
    public int magnetPower;

    public PassDataMagnet(int magnetPower)
    {
        priority = 40;
        this.magnetPower = magnetPower;
    }
}
[System.Serializable]
public class PassDataDamage : PassData
{
    public MyDuo<Element, float> pair; //can't serialize dictionary in inspector

    public PassDataDamage(MyDuo<Element, float> pair)
    {
        this.pair = pair;
        SetPriority();
    }
    public PassDataDamage(Dictionary<Element, float> dict)
    {
        pair = new MyDuo<Element, float>(dict);
        SetPriority();
    }
    public PassDataDamage(Element[] keys, float[] values)
    {
        pair = new MyDuo<Element, float>(keys, values);
        SetPriority();
    }
    void SetPriority() => priority = 50;
}


[System.Serializable]
public class PassDataSpell: PassData
{
    bool IsNone() => effect == HitEffectOnSpell.None;
    [HideIf(nameof(IsNone))] public SpellMain[] spellsToAffect;
    public HitEffectOnSpell effect;

    public PassDataSpell(SpellMain[] spellsToAffect, HitEffectOnSpell effect)
    {
        priority = 100;
        this.spellsToAffect = spellsToAffect;
        this.effect = effect;
    }
}


// [System.Serializable]
// public class PassDataStats : PassData 
// {
//     public MyDuo<Stats, int> pair;
//     public PassDataStats(MyDuo<Stats, int> pair)
//     {
//         priority = 50;
//         this.pair = pair;
//     }
// }
// [System.Serializable]
// public class PassDataTag: PassData
// {
//     public string key;
//     public Dictionary<string, string> tags = new Dictionary<string, string>();
//     public const string TagExecutioner = "Executioner";
//     public const string TagStatusBleed = "Bleeding";
//     public const string TagStatusPoison = "Poisoned";
//     public const string TagStatusBurn = "Burning";
//     public const string TagStatusFreeze = "Freezing";
//     public const string TagStatusJolt = "Jolted";
//
// }

