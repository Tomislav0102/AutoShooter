using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class SoSkill : ScriptableObject
{
    [SerializeField, ReadOnly, FoldoutGroup("Info"), HideLabel] string infoA = "Level of skill starts at 0 (in-game its level1).";
    [SerializeField, TextArea, FoldoutGroup("Info")] string description;
    [SerializeField, TextArea, FoldoutGroup("Info")] string levelUpEffect;
    [Title("General")]
    [EnumButtons] public SkillType skillType;
    [EnumButtons] public AnimAttackType animAttackType;
    public int level;

    [Title("Global")]
    public bool hasStat;
    [ShowIf(nameof(hasStat))] public MyDuo<Stats, int> stats;
    public bool hasExtraDamage;
    [ShowIf(nameof(hasExtraDamage))] public MyDuo<Element, int> extraDamage;
    // public bool hasExtraDamagePercentage;
    // [ShowIf(nameof(hasExtraDamagePercentage))] public MyDuo<Element, float> extraDamagePercentage;

    [Title("Spell")]
    [InfoBox("Does not include stat increases. Extra are stats that affect only this spell (size, duration, knockback...)")]
    public bool hasSpell;
    [ShowIf(nameof(hasSpell))] public SpellMain spell;
    public bool hasExtra;
    bool ShowHasExtra() => hasSpell && hasExtra;
    [ShowIf(nameof(ShowHasExtra))] public MyDuo<Stats, int> extraStats;

    [Title("Generic")]
    [InfoBox("Special use cases, should be used sparingly")]
    public bool hasGeneric; 
    [ShowIf(nameof(hasGeneric))] public int numGeneric;
    [ShowIf(nameof(hasGeneric))] public float floatGeneric;
    [ShowIf(nameof(hasGeneric))] public string stringGeneric;
    // [ShowIf(nameof(hasGeneric))] public int[] numArray;
    // [ShowIf(nameof(hasGeneric))] public float[] floatArray;
    // [ShowIf(nameof(hasGeneric))] public string[] stringArray;

}


