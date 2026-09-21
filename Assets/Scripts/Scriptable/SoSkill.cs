using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Serialization;


[CreateAssetMenu]
public class SoSkill : ScriptableObject
{
    [SerializeField, ReadOnly, FoldoutGroup("Info"), HideLabel] string infoA = "Level of skill starts at 0 (in-game its level1).";
    [SerializeField, TextArea, FoldoutGroup("Info")] string description;
    [SerializeField, TextArea, FoldoutGroup("Info")] string levelUpEffect;
    [Title("General")]
    public SkillName skillName;
    // public string skillNameText;
    // public string levelUpText;
    [EnumButtons] public SkillType skillType;
  //  [EnumButtons] public AnimAttackType animAttackType; //only for enemy animations
    public int level;

    [Title("Global")]
    public bool hasStat;
    [ShowIf(nameof(hasStat))] public StatsGroup[] stats;
    public bool hasExtraDamage;
    [ShowIf(nameof(hasExtraDamage))] public MyDuo<Element, float> extraDamage;
    public bool hasEffect;
    [ShowIf(nameof(hasEffect))] public BuffEffects[] buffEffect;
    
    [Title("Spell")]
    [InfoBox("Extra affect only this spell (PassData format)")]
    public HasSpell hasSpell;
    bool ShowSpell() => hasSpell == HasSpell.Spell;
    bool ShowSpellGroup() => hasSpell == HasSpell.SpellGroup;
    [ShowIf(nameof(ShowSpell))] public SpellMain spell;
    [ShowIf(nameof(ShowSpellGroup))] public SpellGroup spellGroup;
    bool ShowExtra() => hasSpell != HasSpell.None;
    [ShowIf(nameof(ShowExtra))] public bool hasExtra;
    bool ShowHasExtra() => ShowExtra() && hasExtra;
    [ShowIf(nameof(ShowHasExtra))] public PassData passData;
    
    [Title("Generic")]
    [InfoBox("Special use cases, should be used sparingly")]
    public bool hasGeneric; 
    [ShowIf(nameof(hasGeneric))] public int numGeneric;
    [ShowIf(nameof(hasGeneric))] public float floatGeneric;
   // [ShowIf(nameof(hasGeneric))] public string stringGeneric;
    // [ShowIf(nameof(hasGeneric))] public int[] numArray;
    // [ShowIf(nameof(hasGeneric))] public float[] floatArray;
    // [ShowIf(nameof(hasGeneric))] public string[] stringArray;
    
    public enum HasSpell { None, Spell, SpellGroup }
}

