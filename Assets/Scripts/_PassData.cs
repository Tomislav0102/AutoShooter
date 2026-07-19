using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class PassDataContainer
{
    public Brain myBrain;
    public bool canBeBlocked;
    [SerializeReference] public PassData[] data;

    
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
    public int knockBack;
    public Vector2 knockBackDirection; //ignored if = Vector2.zero

    public PassDataKnockBack(int knockBack, Vector2 knockBackDirection = new Vector2())
    {
        priority = 30;
        this.knockBack = knockBack;
        this.knockBackDirection = knockBackDirection;
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
    void SetPriority() => priority = 40;
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
[System.Serializable]
public class PassDataSpell: PassData
{
    public SpellMain[] spellsToAffect;
    public Spell.HitEffectOnSpell effect;

    public PassDataSpell(SpellMain[] spellsToAffect, Spell.HitEffectOnSpell effect)
    {
        priority = 100;
        this.spellsToAffect = spellsToAffect;
        this.effect = effect;
    }
}
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

