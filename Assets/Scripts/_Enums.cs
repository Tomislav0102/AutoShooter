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

public enum MultiShot { AllAtOnce, Consecutive, Random }
public enum EnAttack { Melee, Ranged }

public enum EnAttackRanged
{
    StandardAimed, //normal bullet
    BlindShots, //shooting everywhere, bullet hell
    Other
}
public enum EnMovement { Stationary, Roam, Patrol, Follow, Chase, Flee }