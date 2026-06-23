using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class InjectHealth 
{
    [HideInInspector] public Brain myBrain;
    public bool canBeBlocked;
    [Range(0, 20)] public int knockBack;
    public Dictionary<Element, float> damage =  new Dictionary<Element, float>();

    public Dictionary<string, string> tags = new Dictionary<string, string>();
    public const string TagExecutioner = "Executioner";
    public const string TagStatusBleed = "Bleeding";
    public const string TagStatusPoison = "Poisoned";
    public const string TagStatusBurn = "Burning";
    public const string TagStatusFreeze = "Freezing";
    public const string TagStatusJolt = "Jolted";

    public InjectHealth()
    {
        
    }
    public InjectHealth(Dictionary<Element, float> damage, bool canBeBlocked = false, int knockBack = 0)
    {
        this.damage = damage;
        this.canBeBlocked = canBeBlocked;
        this.knockBack = knockBack;
    }
}