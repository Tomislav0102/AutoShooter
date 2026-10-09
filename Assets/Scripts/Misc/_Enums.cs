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
public enum GenDistance { Closest, Furthest, Middle, Random }
public enum GenTarget { Self, Other }
#endregion

public enum PanelType { Game, Main, Skills, Character, Inventory, LevelUp, Map, Encyclopedia, Options }
public enum BuffType { Added, Percentage, Set }
public enum WeaponType { Slash, Blunt, Pierce }


public enum SkillType { Active, Passive, Ultimate, Replacement }
public enum AnimAttackType
{
    Melee,
    //MeleeBig,
    Ranged,
    //RangedBig,
    //  Cast,
    Ultimate
}
public enum ColliderType { Sphere, Capsule, None }//Capsule always has radius of 0.5f, regardless of areaOfEffect.
public enum DropType { Gold, Xp, Heal, ItemSpell }
public enum Element { Physical, Fire, Ice, Electricity, Poison, Magic }
public enum Faction { GoodGuys, BadGuys, Neutral }
public enum FactionToTarget { Ally, Enemy, All }
public enum Alertness { Relaxed, Alarmed, Fighting }
public enum CombatEvent
{
    Strike, 
    Hit, //brain
    Miss, 
    BeginGetHit, //brain, spell
    GetHit, //brain, spell
    EndGetHit, //brain, spell
    Block, //brain, spell
    Kill, //brain
    Dodge //brain, spell
}
public enum Stats
{
    //primary - derived form primaries
    DamPhysical, 
    DamFire,
    DamIce,
    DamElectricity,
    DamPoison,
    DamMagic,
    MoveSpeed, 
    AttackSpeed,
    Block,
    Dodge,
    Health,
    RegenerationRate, //hp increase (divided by 100) per second
    ResistPhysical, //armor
    ResistFire,
    ResistIce,
    ResistElectricity,
    ResistPoison,
    ResistMagic,
    //secondary - adds to spell variables
    Size, 
    Duration,
    KnockBack,
    CritChance,
    CritMod,
    Projectiles,
    Piercing,
    Ricochet,
    Bounce,
    //tertiary
    Xp,
    Gold,
    Loot,
    SightRange, //just camera zoom -> like in 'Into The Necrovale'
    //quaternary
    ReflexSpells, //all spells except melee and projectiles. If spell can't be reflected that is managed by variable 'reflexCount' in 'SpellMain'
    ReflectMelee, //melee weapons
    ReflectProjectiles, //ranged weapons (not spells), like bows, crossbows, etc...
    ExtraSkillChoice, //default is 3, this number adds to it 
    BodySize
}

public enum SkillName
{
    //shared
    VitalityBoost,
    AegisPlate,
    LastStand,
    PowerSurge,
    Adrenaline,
    EagleEye,
    HeavyHitter,
    Haste,
    Scholar,
    Enlarge,
    RotatingSwords,
    ElementalUpgrade,
    ExtraLife,
    SlowProjectiles,
    Giant,
    Dwarf,
    FreshStart,
    Riposte,
    ExtraSkillChoice,
    Sh01,
    Sh02,
    Sh03,
    Sh04,
    Sh05,
    Sh06,
    Sh07,
    Sh08,
    Sh09,   
    Sh10, 
    //knight
    SpectralRicochet,
    GrandCrescendo,
    ExplosiveHit,
    ElementalStrikes,
    Executioner,
    BleedingStrike,
    MeatDrop,
    AdvanceGuard,
    SweepingArc,
    ArmorBreaker,
    SeismicAnchorage,
    HeavyImpact,
    ConcussiveWave,
    SkullCracker,
    ArcaneHarvest,
    LethargicDomain,
    IronMirror,
    GuardiansValor,
    GuardiansMight,
    SpikedRim,
    ShockwaveBlock,
    IronFortress,
    KnightBase,
    ConcussiveSurge,
    Statue,
    Kn03,
    Kn04,
    Kn05,
    Kn06,
    Kn07,
    Kn08,
    Kn09,
    Kn10,
    //mage
    MageBase,
    ArcaneShield,
    ManaShield,
    BastionPulse,
    Decoy,
    OrbitalBulwark,
    Fireball,
    DragonsBreath,
    MeteorStrike,
    FireNova,
    BlazeTrail,
    Meltdown,
    Pyromania,
    Armageddon,
    ShardWave,
    ChillingTouch,
    IceSpear,
    GlacialShield,
    Shatter,
    AbsoluteZero,
    Blizzard,
    ChainLightning,
    Overload,
    LightningBolt,
    Superconductor,
    StaticCharge,
    Thunderstorm,
    ImprovedMagicMissile,
    OrbOfPower,
    ManaSingularity,
    AetherPulse,
    ArcaneOverflow,
    TimeStop,
    AetherBeam,
    Ma00,
    Ma01,
    Ma02,
    Ma03,
    Ma04,
    Ma05,
    Ma06,
    Ma07,
    Ma08,
    Ma09,
    Ma10,
    //archer
    ArcherBase,
    FrontArrow,
    BackArrow,
    DiagonalArrow,
    ParallelArrow,
    FollowUpArrow,
    Ricochet,
    BouncingArrow,
    PiercingArrow,
    SideArrow,
    FireArrow,
    IceArrow,
    ElectricityArrow,
    PoisonArrow,
    Headshot,
    Clone,
    EvasiveRoll,
    Ar01,
    Ar02,
    Ar03,
    Ar04,
    Ar05,
    Ar06,
    Ar07,
    Ar08,
    Ar09,
    Ar10,
    Ar11,
    Ar12,
    Ar13,
    Ar14,
    Ar15,
    //enemy (maybe redundant)
    //replacement
    ReplacementGold,
    ReplacementHeal
}





