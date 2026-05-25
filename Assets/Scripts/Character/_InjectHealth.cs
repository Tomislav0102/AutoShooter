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
    public object extraData;
    public static string DataExecutioner = "Executioner";
    public static string DataStatusBleed = "Bleeding";

    // public InjectHealth(Transform attacker, Dictionary<Element, float> damage, bool canBeBlocked, int knockBack)
    // {
    //     this.attacker = attacker;
    //     this.damage = damage;
    //     this.canBeBlocked = canBeBlocked;
    //     this.knockBack = knockBack;
    // }
}