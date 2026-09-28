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
    public Sprite icon;
    // public string skillNameText;
    // public string levelUpText;
    [EnumButtons] public SkillType skillType;
  //  [EnumButtons] public AnimAttackType animAttackType; //only for enemy animations
    public int level;

    [Title("Global")]
    [InfoBox("Affect user of skill")]
    public bool hasStat;
    [ShowIf(nameof(hasStat))] public BuffStats[] stats;
    public bool hasExtraDamage;
    [ShowIf(nameof(hasExtraDamage))] public BuffType damageType;
    [ShowIf(nameof(hasExtraDamage))] public MyDuo<Element, float> extraDamage;
    public bool hasEffect;
    [ShowIf(nameof(hasEffect))] public BuffEffects[] buffEffect;
    
    [Title("Spell")]
    [InfoBox("Extra affect target of spell (PassData format)")]
    public HasSpell hasSpell;
    bool ShowSpell() => hasSpell == HasSpell.Spell;
    bool ShowSpellGroup() => hasSpell == HasSpell.SpellGroup;
    [ShowIf(nameof(ShowSpell))] public SpellMain spell;
    [ShowIf(nameof(ShowSpellGroup))] public SpellGroup spellGroup;
    bool ShowPd() => hasSpell != HasSpell.None;
    [ShowIf(nameof(ShowPd))] public bool hasPassData;
    bool ShowHasPd() => ShowPd() && hasPassData;
    [ShowIf(nameof(ShowHasPd))] public PassData passData;
    [ShowIf(nameof(ShowPd))] public bool hasAfterSpells;
    bool ShowHasAfters() => hasAfterSpells == true && ShowPd();
    [ShowIf(nameof(ShowHasAfters))] public SpellMain[] afterSpells;
    [Title("Generic")]
    [InfoBox("Special use cases. Array is used when more that one value is needed.")]
    public bool hasGeneric; 
    [FormerlySerializedAs("floatGeneric")] [ShowIf(nameof(hasGeneric))] public float valueGeneric;
    [ShowIf(nameof(hasGeneric))] public float[] valueGenericArray;
    
    public enum HasSpell { None, Spell, SpellGroup }
}

