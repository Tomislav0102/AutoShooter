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
    public float dashTime = 0.2f;
    public int dashPower = 50;
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
    // [Title("Status")]
    // [SerializeField] StatusGroup[] status;
    // [System.Serializable] 
    // public struct StatusGroup
    // {
    //     public Status.Effect effect;
    //     public string name;
    //     public Sprite sprite;
    // }

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

