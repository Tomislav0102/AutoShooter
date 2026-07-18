using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class PassData { }


[System.Serializable]
public class PassDataShared : PassData
{
    public Brain myBrain;
    public bool canBeBlocked;

    public PassDataShared(Brain myBrain, bool canBeBlocked = false)
    {
        this.myBrain = myBrain;
        this.canBeBlocked = canBeBlocked;
    }
}
[System.Serializable]
public class PassDataDamage : PassData
{
    public MyDuo<Element, float> pair; //can't serialize dictionary in inspector

    public PassDataDamage(MyDuo<Element, float> pair)
    {
        this.pair = pair;
    }
}
[System.Serializable]
public class PassDataKnockBack : PassData
{
    public int knockBack;
    public Vector2 knockBackDirection; //ignored if = Vector2.zero

    public PassDataKnockBack(int knockBack, Vector2 knockBackDirection = new Vector2())
    {
        this.knockBack = knockBack;
        this.knockBackDirection = knockBackDirection;
    }
}
[System.Serializable]
public class PassDataManaShield : PassData
{
    public int manaShieldPoints;

    public PassDataManaShield(int manaShieldPoints)
    {
        this.manaShieldPoints = manaShieldPoints;
    }
}
[System.Serializable]
public class PassDataStats : PassData
{
    public MyDuo<Stats, int> pair;
    public PassDataStats(MyDuo<Stats, int> pair)
    {
        this.pair = pair;
    }

}
[System.Serializable]
public class PassDataSpell: PassData
{
    public SpellMain[] spellsToAffect;
    public Spell.HitEffectOnSpell effect;

    public PassDataSpell(SpellMain[] spellsToAffect, Spell.HitEffectOnSpell effect)
    {
        this.spellsToAffect = spellsToAffect;
        this.effect = effect;
    }
}
[System.Serializable]
public class PassDataTag: PassData
{
    public string key;
    public Dictionary<string, string> tags = new Dictionary<string, string>();
    public const string TagExecutioner = "Executioner";
    public const string TagStatusBleed = "Bleeding";
    public const string TagStatusPoison = "Poisoned";
    public const string TagStatusBurn = "Burning";
    public const string TagStatusFreeze = "Freezing";
    public const string TagStatusJolt = "Jolted";

}

