using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterJumpDetectAbility : CharacterDangling
{

    public static System.Action<bool> ShowJumpNotification;

    private CharacterDangling _dangling;

    protected override void Initialization()
    {
        base.Initialization();
        _dangling = GetComponent<CharacterDangling>();
    }

    protected override void Dangling()
    {
        if ((_movement.CurrentState == CharacterStates.MovementStates.Jumping)
        || (_movement.CurrentState == CharacterStates.MovementStates.Falling)
        || !_character._state.IsGrounded)
        {
            SetDanglingStatus(true);
            return;
        }

        if (_dangling != null && _dangling.dangling)
        {
            SetDanglingStatus(false);
            return;
        }

        if (drawGizmo)
        {
            Debug.DrawRay(transform.position + DanglingRaycastOrigin1, -transform.up * DanglingRaycastLength, rayColor);
            Debug.DrawRay(transform.position + DanglingRaycastOrigin2, -transform.up * DanglingRaycastLength, rayColor);
        }

        HitRayCast();
    }

    protected override void SetDanglingStatus(bool hit)
    {
        if (hit)
        {
            if (dangling)
            {
                dangling = false;
                ShowJumpNotification?.Invoke(false);
            }
        }
        else
        {
            if (!dangling)
            {
                ShowJumpNotification?.Invoke(true);
            }

            dangling = true;
        }
    }
}
