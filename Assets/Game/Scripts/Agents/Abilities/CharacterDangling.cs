using UnityEngine;
using System.Collections;
using GodSpeedGames.Tools;


public class CharacterDangling : CharacterAbility
{
    public bool drawGizmo = false;
    public LayerMask groundMask;

    [Header("Dangling")]
    /// the origin of the raycast used to detect pits. This is relative to the transform.position of our character
    /// We must take 2 raycast point to correctly detect whole in the ground
    public Vector3 DanglingRaycastOrigin1 = new Vector3(0.7f, -0.25f, 0f);
    public Vector3 DanglingRaycastOrigin2 = new Vector3(0.7f, -0.25f, 0f);
    /// the length of the raycast used to detect pits
    public float DanglingRaycastLength = 2f;
    public Color rayColor;

    protected Vector3 _leftOne = new Vector3(-1, 1, 1);

    [ReadOnly] public bool dangling = false;

    RaycastHit[] _hitinfo = new RaycastHit[1];

    public override void ProcessAbility()
    {
        base.ProcessAbility();

        if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
            Dangling();
    }

    protected virtual void Dangling()
    {
        if (!_character._state.IsGrounded && (_movement.CurrentState == CharacterStates.MovementStates.Dangling))
        {
            _movement.ChangeState(CharacterStates.MovementStates.Falling);
        }

        if (!AbilityPermitted
        || (_movement.CurrentState == CharacterStates.MovementStates.Jumping)
        || (_movement.CurrentState == CharacterStates.MovementStates.Falling)
        || (_movement.CurrentState == CharacterStates.MovementStates.Idle)
        || !_character._state.IsGrounded)
        {
            return;
        }

        if (drawGizmo)
        {
            Debug.DrawRay(transform.position + DanglingRaycastOrigin1, -transform.up * DanglingRaycastLength, rayColor);
            Debug.DrawRay(transform.position + DanglingRaycastOrigin2, -transform.up * DanglingRaycastLength, rayColor);
        }

        HitRayCast();
    }

    protected virtual void HitRayCast() {
        int len = Physics.RaycastNonAlloc(transform.position + DanglingRaycastOrigin1, -transform.up, _hitinfo, DanglingRaycastLength, groundMask);
        if (len > 0)
        {
            SetDanglingStatus(true);
        }
        else
        {
            len = Physics.RaycastNonAlloc(transform.position + DanglingRaycastOrigin2, -transform.up, _hitinfo, DanglingRaycastLength, groundMask);
            if (len > 0)
            {
                SetDanglingStatus(true);
            }
            else
                SetDanglingStatus(false);
        }
    }


    protected virtual void SetDanglingStatus(bool hit)
    {
        if (hit)
        {
            dangling = false;
        }
        else
        {
            if (!dangling)
            {
                StopCoroutine("DanglingAnimation");
                StartCoroutine("DanglingAnimation");
            }

            dangling = true;
        }
    }

    IEnumerator DanglingAnimation()
    {
        _movement.ChangeState(CharacterStates.MovementStates.Dangling);
        _character._animator.SetTrigger("Dangling");
        yield return new WaitForSeconds(1.0f);
        _movement.ChangeState(CharacterStates.MovementStates.Idle);
    }
}
