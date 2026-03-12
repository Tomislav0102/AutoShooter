#region Gens
public enum GenOrder { Primary, Secondary, Tertiary }
public enum GenSize { Small, Medium, Big }
public enum GenDirection { Enter, Exit }
public enum GenSide { Left, Right } 
public enum GenActivation {On, Off}
public enum GenConfirm { Yes, No }
public enum GenMenuControl {Open, Close, Toggle }
public enum GenResult { Win, Lose, Draw }
public enum GenChange { Increase, Decrease }
#endregion
public enum Element { Physical, Fire, Ice, Electricity, Poison, Force, Magic }

public enum Faction { Ally, Foe, Neutral }
public enum StatsPrimary { Strength, Dexterity, Constitution, Intelligence }
public enum StatsSecondary { MeleeDamage, RangedDamage, MoveSpeed, AttackSpeed, PhysicalResistance, MagicDamage, MagicalResistance }

public enum Buff
{
    Slowed, //attack and move speed
    Rooted, //move speed is 0, attack speed unaffected
    Stunned, //completely passive, enemy does nothing
    Confused, //attacks random character, changes target often, does not respond to aggro 
    Blinded, //like confused, but only close target
    Charmed //behaves like summon
}

