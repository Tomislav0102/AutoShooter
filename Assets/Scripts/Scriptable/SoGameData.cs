using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class SoGameData : ScriptableObject
{
    [Title("Dynamic")] public bool showParticles = true;
    public bool showFloatingInfo = true;
    [Title("General")] public float dashTime = 0.2f;
    public int dashPower = 50;
    public int stunDurationBase = 3;
    public float agentRotSpeed;

    [TabGroup("Elements")] [SerializeField]
    ElementGroup[] element;

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

    [TabGroup("Colors")] public Color colHeal,
        colBleed;


    [TabGroup("Strings")] public string prefsEnergyStartTime,
        prefsEnergyFinishTime,
        prefsTestEnemyCount,
        prefsTestChosenPlayer,
        sceneMain,
        sceneGame,
        sceneLevel,
        layActors,
        layObstacle,
        laySpell,
        laySpellInterrupt;
}



