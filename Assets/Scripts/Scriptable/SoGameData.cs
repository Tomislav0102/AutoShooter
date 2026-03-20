using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class SoGameData : SerializedScriptableObject
{
    [Title("General")]
    public float pushDuration;
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

