using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class SoGameData : ScriptableObject
{
    [Title("Dynamic")] 
    public int level;
    [Title("General")]
    public float rofSpells;
    public float pushDuration;
    public Material matSeeThroughWalls;
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

    [Title("Colors")]
    public Color colHeal;
    public Color colBleed;

    [Title("Strings")]
    public string prefsEnergyStartTime;
    public string prefsEnergyFinishTime;
    public string sceneMain, sceneGame;
    public string SceneLevel() => $"Level{level}";
    public string layActors;
    public string layGround;
    public string layObstacle;
    public string layWallsSeeThrough;
    public string laySpell;
    public string laySpellInterrupt;
    

    
    
    
    
    
    
    
    
    
    
    [System.Serializable]
    public struct ElementGroup
    {
        public Element element;
        public string name;
        public Color col;
        public Sprite sprite;
        public TMP_ColorGradient gradient;
    }
}

