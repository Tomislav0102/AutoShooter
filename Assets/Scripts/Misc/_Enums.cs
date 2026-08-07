#region Gens
public enum GenOrder { Primary, Secondary, Tertiary }
public enum GenSize { Small, Medium, Big }
public enum GenDirection { Enter, Exit }
public enum GenSide { Left, Center, Right }
public enum GenActivation { On, Off }
public enum GenConfirm { Yes, No }
public enum GenMenuControl { Open, Close, Toggle }
public enum GenResult { Win, Lose, Draw }
public enum GenChange { Add, Remove }
public enum GenCalcChange { Increase, Decrease, Set, None }
public enum GenDistance { Closest, Furthest, Middle, Random }
#endregion

public enum AnimAttackType
{
    Melee,
    //MeleeBig,
    Ranged,
    //RangedBig,
    //  Cast,
    Ultimate
}
public enum ColliderType { Sphere, Capsule, None }//includes Overlap shape too. Capsule always has radius of 0.5f, regardless of areaOfEffect.
public enum DropType { Gold, Xp, Heal, ItemSpell }
public enum Element { Physical, Fire, Ice, Electricity, Poison, Force, Magic }
public enum Faction { GoodGuys, BadGuys, Neutral }
public enum FactionToTarget { Ally, Enemy, All }
public enum Alertness { Relaxed, Alarmed, Fighting }

public enum CombatEvent { Strike, Hit, Miss, GetHit, Block, Kill }
public enum Stats
{
    //primary
    Strength, 
    Dexterity, 
    Constitution, 
    Intelligence,
    
    //secondary - derived form primaries
    MeleeDamage, 
    RangedDamage, 
    MagicDamage, 
    MoveSpeed, 
    AttackSpeed,
    Block,
    Dodge,
    Health,
    RegenerationRate, //hp increase (divided by 100) per second
    Toughness,
    Resolve,
    Resistances, //opens another enum 'Element'
    
    //tertiary - adds to spell variables
    Size, 
    Area,
    Duration,
    KnockBack,
    CritChance,
    CritMod,
    
    //quaternary
    Xp,
    Gold,
    Loot
    //quinary
}





