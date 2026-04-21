#region Gens
public enum GenOrder { Primary, Secondary, Tertiary }
public enum GenSize { Small, Medium, Big }
public enum GenDirection { Enter, Exit }
public enum GenSide { Left, Right }
public enum GenActivation { On, Off }
public enum GenConfirm { Yes, No }
public enum GenMenuControl { Open, Close, Toggle }
public enum GenResult { Win, Lose, Draw }
public enum GenChange { Add, Remove }
#endregion
public enum Element { Physical, Fire, Ice, Electricity, Poison, Force, Magic }
public enum Faction { GoodGuys, BadGuys }
public enum FactionTarget { Ally, Enemy, All }
public enum Disposition { Relaxed, Wary, Fighting }

public enum Stats
{
    Strength, 
    Dexterity, 
    Constitution, 
    Intelligence,
    //
    MeleeDamage, 
    RangedDamage, 
    MagicDamage, 
    MoveSpeed, 
    AttackSpeed,
    Block,
    Dodge,
    Health,
    Toughness,
    Resolve,
    ///
    CritChance,
    CritMod,
    Xp,
    Gold,
    Loot,
    Resistances //opens another enum 'Element'
}




