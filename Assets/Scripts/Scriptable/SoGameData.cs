using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class SoGameData : ScriptableObject
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

    [Title("Strings")]
    public string prefsEnergyStartTime;
    public string prefsEnergyFinishTime;
    
    [Title("Factions, layers")]
    public LayerMask laySpells;
    public FactionGroup[] factionGroup;
    public bool CanTargetFaction(Faction myFaction, Faction target, FactionTarget targetFaction)
    {
        switch (targetFaction)
        {
            case FactionTarget.Ally:
                return myFaction == target;
            case FactionTarget.Enemy:
                return myFaction != target;
            case FactionTarget.All:
                return true;
        }
        return false;
    }

    public LayerMask TargetLayer(Faction myFaction, FactionTarget targetFaction)
    {
        int f = (int)myFaction;
        f = (1 + f) % 2;
        Faction oppositeFaction = (Faction)f;
        switch (targetFaction)
        {
            case FactionTarget.Ally:
                switch (myFaction)
                {
                    case Faction.GoodGuys:
                        return LayerMaskFromFaction(oppositeFaction);
                    case Faction.BadGuys:
                        return LayerMaskFromFaction(myFaction);                        break;
                }
                break;
            case FactionTarget.Enemy:
                switch (myFaction)
                {
                    case Faction.GoodGuys:
                        return LayerMaskFromFaction(myFaction);
                    case Faction.BadGuys:
                        return LayerMaskFromFaction(oppositeFaction);                        break;
                }
                return default;
            case FactionTarget.All:
                return LayerMaskFromFaction(Faction.GoodGuys) | LayerMaskFromFaction(Faction.BadGuys);
        }

        return default;
        
        LayerMask LayerMaskFromFaction(Faction faction)
        {
            foreach (FactionGroup group in factionGroup)
            {
                if (group.faction == faction) return group.layerMask;
            }
            return default;
        }

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

    [System.Serializable]
    public struct FactionGroup
    {
        public Faction faction;
        public LayerMask layerMask;
    }
}

