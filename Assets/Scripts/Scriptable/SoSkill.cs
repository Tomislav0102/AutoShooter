using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Serialization;


[CreateAssetMenu]
public class SoSkill : ScriptableObject
{
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
    
    [Title("Specific")]
    [InfoBox("Any change to spell variables are in 'passData' variable (e.g. damage, knockback, size, numOfProjectiles).")]
    public HasSpell hasSpell;
    bool ShowSpell() => hasSpell == HasSpell.Spell;
    bool ShowSpellGroup() => hasSpell == HasSpell.Group;
    [ShowIf(nameof(ShowSpell))] public SpellMain spell;
    [ShowIf(nameof(ShowSpellGroup))] public SpellGroup spellGroup;
    bool ShowSpellInfo() => hasSpell != HasSpell.None;
    public bool hasPassData;
    [ShowIf(nameof(hasPassData))] public PassData passData;
    [ShowIf(nameof(ShowSpellInfo))] public bool hasAfterSpells;
    bool ShowHasAfters() => hasAfterSpells == true && ShowSpellInfo();
    [ShowIf(nameof(ShowHasAfters))] public SpellMain[] afterSpells;
    [Title("Generic")]
    [InfoBox("Special use cases. Array is used when more that one value is needed.")]
    public bool hasGeneric; 
    [ShowIf(nameof(hasGeneric))] public float valueGeneric;
    [ShowIf(nameof(hasGeneric))] public float[] valueGenericArray;
    
    public enum HasSpell { None, Spell, Group }
    
}

