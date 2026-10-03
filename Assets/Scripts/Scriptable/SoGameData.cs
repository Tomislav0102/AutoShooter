using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class SoGameData : ScriptableObject
{
    [Title("Dynamic")]
    public bool showParticles = true;
    public bool showFloatingInfo= true;
    // [Title("Dynamic enemy modifiers")]
    // public MyDuo<Status.Effect, int> enUnderEffect;
    // public MyDuo<Status.Effect, float> enDamMod; //used by one Knight skill
    // [Button]
    // void SetEnDamageModifiers()
    // {
    //     enUnderEffect = new MyDuo<Status.Effect, int>();
    //     enDamMod = new MyDuo<Status.Effect, float>();
    //     int length = System.Enum.GetNames(typeof(Status.Effect)).Length;
    //     for (int i = 0; i < length; i++)
    //     {
    //         enDamMod.Add((Status.Effect)i, 1f);
    //         enUnderEffect.Add((Status.Effect)i, 0);
    //     }
    // }
   // public float enBurnArmorReduction = 1f; //used by one Mage skill
    [Title("General")]
    public float rofSpells;
    public float dashTime = 0.2f;
    public int dashPower = 50;
    public int stunDurationBase = 3;
    [Title("Navigation")]
    public float agentRotSpeed;
    
    [Title("Elements")]
    [SerializeField] ElementGroup[] element;
    public ElementGroup GetElement(Element el)
    {
        foreach (ElementGroup item in element)
        {
            if (item.element == el) return item;
        }
        return default;
    }
     [System.Serializable] 
     public struct ElementGroup
    {
        public Element element;
        public string name;
        public Color col;
        public Sprite sprite;
        public TMP_ColorGradient gradient;
    }

    [Title("Colors")]
    public Color colHeal;
    public Color colBleed;

    
    [Title("Strings")]
    public string prefsEnergyStartTime;
    public string prefsEnergyFinishTime;
    public string prefsTestEnemyCount;
    public string prefsTestChosenPlayer;
    public string sceneMain, sceneGame, sceneLevel;
    public string layActors;
    public string layObstacle;
    public string laySpell;
    public string laySpellInterrupt;
}



