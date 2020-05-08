using GodSpeedGames.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVerticalMovement : CharacterAbility
{
    public bool canJump = true;
    [Tooltip("Desired jump height while jumping")]
    [Condition("canJump", true)]
    public float jumpHeight = 2.3f;

    protected float VerticalSpeed { get; set; }               // How fast Player is currently moving up or down.
    protected bool _gotJumpMove = false;

    private bool _makeHighJump = false;

    const float k_StickingGravityProportion = 0.3f;

    public bool JumpPressed
    {
        get
        {
            return _gotJumpMove;
        }
    }


    public override void ProcessAbility()
    {
        base.ProcessAbility();

        CalculateVerticalMovement();
    }

    protected override void Initialization()
    {
        base.Initialization();
        Reset();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        PlayerDetector.InsideJumpSafeArea += InsideJumpSafeArea;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PlayerDetector.InsideJumpSafeArea -= InsideJumpSafeArea;
    }

    protected virtual void CalculateVerticalMovement()
    {
        if (AbilityPermitted)
        {
            if (_character._state.IsGrounded)
            {
                //VerticalSpeed = -_character.gravity * k_StickingGravityProportion;

                // If jump is held
                if (_gotJumpMove)
                {
                    SetVerticleSpeed(CalculateJumpVerticalSpeed());
                    _gotJumpMove = false;
                }

                if (Mathf.Approximately(VerticalSpeed, 0f))
                {
                    VerticalSpeed = 0f;
                }

                if (_character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead)
                {
                    if (_character._state.JustGotGrounded && _movement.CurrentState != CharacterStates.MovementStates.Idle && _character.Speed.z == 0)
                    {
                        _movement.ChangeState(CharacterStates.MovementStates.Idle);
                    }
                }
            }
            else
            {
                //VerticalSpeed = _character.Speed.y;
                // If a jump is approximately peaking, make it absolute.
                if (Mathf.Approximately(VerticalSpeed, 0f))
                {
                    VerticalSpeed = 0f;
                }

                VerticalSpeed -= _character.gravity * Time.deltaTime;

                if (_character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead)
                {
                    if (VerticalSpeed < .2f && _movement.CurrentState != CharacterStates.MovementStates.Falling)
                        _movement.ChangeState(CharacterStates.MovementStates.Falling);
                    else if (VerticalSpeed > .2f && _movement.CurrentState != CharacterStates.MovementStates.Jumping)
                        _movement.ChangeState(CharacterStates.MovementStates.Jumping);
                }
            }

            _character.SetVerticalMovement(VerticalSpeed);
        }
    }

    public void SetVerticleSpeed(float speed)
    {
        VerticalSpeed = speed;
        _character._state.IsGrounded = false;
        _movement.ChangeState(CharacterStates.MovementStates.Jumping);
        // we start our sounds
        PlayAbilityStartSfx();
    }

    protected override void InitializeAnimatorParameters()
    {
        RegisterAnimatorParameter("Jumping", AnimatorControllerParameterType.Bool);
        RegisterAnimatorParameter("Falling", AnimatorControllerParameterType.Bool);
    }

    public override void UpdateAnimator()
    {
        GSGAnimator.UpdateAnimatorBool(_animator, "Jumping", ( _condition.CurrentState == CharacterStates.CharacterConditions.Normal &&
            _movement.CurrentState == CharacterStates.MovementStates.Jumping ), _character._animatorParameters);

        GSGAnimator.UpdateAnimatorBool(_animator, "Falling", ( _condition.CurrentState == CharacterStates.CharacterConditions.Normal &&
            _movement.CurrentState == CharacterStates.MovementStates.Falling ), _character._animatorParameters); // no falling animation
    }

    public override void Reset()
    {
        base.Reset();
        _makeHighJump = false;
        _gotJumpMove = false;
        VerticalSpeed = 0;
        if (_movement.CurrentState == CharacterStates.MovementStates.Jumping)
            _movement.ChangeState(CharacterStates.MovementStates.Falling);
    }

    protected override void OnGotMove(Move currentMove)
    {
        if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
        {
            if (canJump)
            {
                if (currentMove == Move.Jump)
                {
                    _gotJumpMove = true;
                }
            }
        }
    }

    protected virtual float CalculateJumpVerticalSpeed(bool isDouble = false)
    {
        if (_makeHighJump)
            return Mathf.Sqrt(4 * jumpHeight * _character.gravity);

        return Mathf.Sqrt(2 * jumpHeight * _character.gravity);
    }

    public void DisableJump()
    {
        AbilityPermitted = false;
    }

    public void EnableJump()
    {
        AbilityPermitted = true;
    }

    void InsideJumpSafeArea(bool safe)
    {
        // StopCoroutine("WaitForGroundded");

        if (safe)
        {
            //EnableJump();
            _makeHighJump = false;
        }
        else
        {
            _makeHighJump = true;
            /*if (_character._state.IsGrounded)
            {
                DisableJump();
            }
            else
            {
                StartCoroutine("WaitForGroundded");
            }*/
        }
    }

    IEnumerator WaitForGroundded()
    {
        while (true)
        {
            if (_character._state.IsGrounded)
            {
                Debug.Log("WaitForGroundded");
                DisableJump();
                StopCoroutine("WaitForGroundded");
                yield break;
            }

            yield return null;
        }
    }
}
