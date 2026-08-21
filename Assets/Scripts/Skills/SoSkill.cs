using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class SoSkill : ScriptableObject
{
    [Title("General")]
    public string skillName;
    public SoSkill[] requirements;
    public bool usedByKnight;
    public bool usedByMage;
    public bool usedByArcher;

    [Title("Global")]
    [InfoBox("Applies to all spells. Integers are added, floats are multiplied.")]
    public bool hasExtraProjectiles;
    [ShowIf(nameof(hasExtraProjectiles))] public int extraProjectileCount;
    public bool hasExtraSize;
    [ShowIf(nameof(hasExtraSize))] public float extraSize;
    public bool hasExtraDuration;
    [ShowIf(nameof(hasExtraDuration))] public float extraDuration;
    public bool hasExtraDamage;
    [ShowIf(nameof(hasExtraDamage))] public MyDuo<Element, int> extraDamage;
    // public bool hasExtraDamagePercentage;
    // [ShowIf(nameof(hasExtraDamagePercentage))] public MyDuo<Element, float> extraDamagePercentage;

    [Title("Spell")]
    [InfoBox("includes all stat increases (more Hp, more Crits etc.)")]
    public bool hasSpell;
    [ShowIf(nameof(hasSpell))] public SpellMain spell;
    [ShowIf(nameof(hasSpell))] public bool hasSpellProjectileCount;
    bool ShowSpellProjCount() => hasSpell && hasSpellProjectileCount;
    [ShowIf(nameof(ShowSpellProjCount))] public int spellProjectileCount = 1;

    [Title("Combat events")]
    [InfoBox("needs to be more detailed (knights skills)")]
    public bool hasCombatEventReq;
    [ShowIf(nameof(hasCombatEventReq))] public CombatEvent combatEventReq;

    [Title("Generic")]
    [InfoBox("special use cases, should be used sparingly")]
    public int[] nums;
    public float[] floats;
    public string[] strings;

}


