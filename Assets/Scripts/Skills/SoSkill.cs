using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class SoSkill : ScriptableObject
{
    [InfoBox("Integers are added, floats are multiplied.")]
    [Title("General")]
    public string skillName;
    [EnumButtons] public SkillType skillType;


    [Title("Global")]
    public bool hasStat;
    [ShowIf(nameof(hasStat))] public MyDuo<Stats, int> stats;
    public bool hasExtraDamage;
    [ShowIf(nameof(hasExtraDamage))] public MyDuo<Element, int> extraDamage;
    // public bool hasExtraDamagePercentage;
    // [ShowIf(nameof(hasExtraDamagePercentage))] public MyDuo<Element, float> extraDamagePercentage;

    [Title("Spell")]
    [InfoBox("Does not include stat increases. Extra are stats that affect this spell (size, duration, knockback...")]
    public bool hasSpell;
    [ShowIf(nameof(hasSpell))] public SpellMain spell;
    public bool hasExtra;
    bool ShowHasExtra() => hasSpell && hasExtra;
    [ShowIf(nameof(ShowHasExtra))] public MyDuo<Stats, int> extraStats;

    [Title("Generic")]
    [InfoBox("Special use cases, should be used sparingly")]
    public bool hasGeneric; 
    [ShowIf(nameof(hasGeneric))] public int[] nums;
    [ShowIf(nameof(hasGeneric))] public float[] floats;
    [ShowIf(nameof(hasGeneric))] public string[] strings;

}


