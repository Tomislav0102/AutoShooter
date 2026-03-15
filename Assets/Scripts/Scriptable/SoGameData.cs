using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class SoGameData : SerializedScriptableObject
{
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
        public TMP_ColorGradient gradient;
    }    
}

