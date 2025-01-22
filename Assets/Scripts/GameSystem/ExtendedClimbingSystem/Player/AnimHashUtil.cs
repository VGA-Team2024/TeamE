using UnityEngine;

public static class AnimHashUtil
{
    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int IsGround = Animator.StringToHash("IsGround");
    public static readonly int InputX = Animator.StringToHash("InputX");
    public static readonly int InputY = Animator.StringToHash("InputY");
    public static readonly int Direction = Animator.StringToHash("Direction");
    public static readonly int ClimbJump = Animator.StringToHash("ClimbJump");
    public static readonly int FallDistance = Animator.StringToHash("FallDistance");
    public static readonly int IsClimb = Animator.StringToHash("IsClimb");
    public static readonly int Attack = Animator.StringToHash("Attack");
    public static readonly int Stab = Animator.StringToHash("Stab");
    public static readonly int Charge = Animator.StringToHash("ArrowCharge");
    public static readonly int Release = Animator.StringToHash("ArrowRelease");
}