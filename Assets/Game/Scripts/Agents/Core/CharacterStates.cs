using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStates
{
    public enum CharacterConditions
    {
        Normal,
        Frozen,
        Paused,
        Dead,
        Falling,
        Won
    }

    public enum MovementStates
    {
        Null,
        Idle,
        Walking,
        Falling,
        Running,
        WalkingOnSlipery,
        StopRunning,
        Slipping,
        Jumping,
        Bending,
        DoubleJumping,
        RightMove,
        LeftMove,
        Dangling
    }

}
