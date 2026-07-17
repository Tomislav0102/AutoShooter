using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class InjectHealth
{
    [HideInInspector] public Brain myBrain;
    public bool canBeBlocked;
    
    //knockback, push
    [Range(0, 20), GUIColor("orange")] public int knockBack;
    [GUIColor("orange")] public Vector2 knockBackDirection; //ignored if = Vector2.zero
    //mana shield, extra hp
    [Range(0, 1000), GUIColor("blue")]public int manaShieldPoints;
    //just damage
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
        damage = new Dictionary<Element, float>();
    }
    public InjectHealth(Dictionary<Element, float> damage, bool canBeBlocked = false, int knockBack = 0)
    {
        this.damage = damage;
        if (this.damage == null) this.damage = new Dictionary<Element, float>();
        this.canBeBlocked = canBeBlocked;
        this.knockBack = knockBack;
    }
}