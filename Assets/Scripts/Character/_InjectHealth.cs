using UnityEngine;

[System.Serializable]
public class InjectHealth
{
    [HideInInspector] public Transform attacker;
    [HideInInspector] public float damage;
    public bool canBeBlocked;
    [Range(0, 20)] public int knockBack;
    public Element element;

    public InjectHealth(Transform attacker, float damage, bool canBeBlocked, int knockBack, Element element)
    {
        this.attacker = attacker;
        this.damage = damage;
        this.canBeBlocked = canBeBlocked;
        this.knockBack = knockBack;
        this.element = element;
    }
}