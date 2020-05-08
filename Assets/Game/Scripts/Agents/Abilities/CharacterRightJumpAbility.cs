using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRightJumpAbility : CharacterAbility
{
    public bool drawGizmo = false;
    public LayerMask groundMask;

    [Header("Dangling")]
    /// the origin of the raycast used to detect pits. This is relative to the transform.position of our character
    /// We must take 2 raycast point to correctly detect whole in the ground
    public Vector3 DanglingRaycastOrigin1 = new Vector3(0.7f, -0.25f, 0f);
    public Vector3 DanglingRaycastOrigin2 = new Vector3(0.7f, -0.25f, 0f);

    [Tooltip("Distance for tracking jump afte hole detection")]
    public float distance = 2f;

    /// the length of the raycast used to detect pits
    public float DanglingRaycastLength = 2f;

    public Color holeDetecRayColor;
    public Color distanceCheckRayColor;

    protected CharacterVerticalMovement _jumpAbility;

    [SerializeField] private bool _holeDetected = false;
    private float _zPositionWhenHoleDetcted;
    private float _distanceCovered;

    protected override void Initialization()
    {
        base.Initialization();
        _jumpAbility = GetComponent<CharacterVerticalMovement>();
    }

    public override void EarlyProcessAbility()
    {
        base.EarlyProcessAbility();
        DetectHole();
    }

    /*public override void ProcessAbility()
    {
        if (drawGizmo)
        {
            Debug.DrawRay(transform.position + Vector3.forward * distance, -transform.up * DanglingRaycastLength, distanceCheckRayColor);
            Debug.DrawRay(transform.position + DanglingRaycastOrigin1, -transform.up * DanglingRaycastLength, holeDetecRayColor);
            Debug.DrawRay(transform.position + DanglingRaycastOrigin2, -transform.up * DanglingRaycastLength, holeDetecRayColor);
        }

        base.ProcessAbility();
        if (!_holeDetected)
            DetectHole();
    }*/

    protected virtual void DetectHole()
    {
        if (!AbilityPermitted
        || (_movement.CurrentState == CharacterStates.MovementStates.Jumping)
        || (_movement.CurrentState == CharacterStates.MovementStates.Falling)
        || !_character._state.IsGrounded)
        {
            return;
        }

        if (drawGizmo)
        {
            Debug.DrawRay(transform.position + Vector3.forward * distance, -transform.up * 3, distanceCheckRayColor);
            Debug.DrawRay(transform.position + DanglingRaycastOrigin1, -transform.up * DanglingRaycastLength, holeDetecRayColor);
            Debug.DrawRay(transform.position + DanglingRaycastOrigin2, -transform.up * DanglingRaycastLength, holeDetecRayColor);
        }

        if (_holeDetected)
        {
            _distanceCovered = Mathf.Abs(transform.position.z - _zPositionWhenHoleDetcted);
            if (_distanceCovered <= distance)
            {
                // check for jump input in this distance
                if (_jumpAbility.JumpPressed)
                    _jumpAbility.DisableJump();
            }
            else
            {
                _holeDetected = false;
                _jumpAbility.EnableJump();
            }
        }
        else
        {
            if (!Physics.Raycast(transform.position + DanglingRaycastOrigin1, -transform.up, out RaycastHit hitinfo1, DanglingRaycastLength, groundMask) &&
                !Physics.Raycast(transform.position + DanglingRaycastOrigin2, -transform.up, out RaycastHit hitinfo2, DanglingRaycastLength, groundMask))
            {
                HoleDetected(true);
            }
        }
    }

    protected virtual void HoleDetected(bool detect)
    {
        _holeDetected = detect;
        _zPositionWhenHoleDetcted = transform.position.z;
    }
}
