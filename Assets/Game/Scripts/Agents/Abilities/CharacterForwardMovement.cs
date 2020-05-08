using GodSpeedGames.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterForwardMovement : CharacterAbility
{
    public GameDataTracker tracker;
    /// the current reference movement speed
    public float MovementSpeed { get; set; }

    public float forwardMovementSpeed;

    [Tooltip("Check this true if Player needs to run/walk continuously")]
    public bool continousForwardMovement = false;

    [Tooltip("If player needs to go forward while jumping, then check this true.")]
    public bool moveForwardWhileJumping;
    [Condition("moveForwardWhileJumping", true)]
    public float forwardMovementSpeedWhileJumping = 0;

    protected float m_forwardMovementInput = 0;
    protected bool _onSliperryPlatform;

    const float k_GroundAcceleration = 20f;
    const float k_GroundDeceleration = 25f;

    private int _initialZPosition;
    private int _distanceCovered;
    private bool _justGotGrounded = false;

    protected override void Initialization()
    {
        base.Initialization();

        if (continousForwardMovement)
            m_forwardMovementInput = 1;

        _initialZPosition = (int)transform.position.z;
        _distanceCovered = 0;
        tracker.distanceCovered = 0;
    }

    public override void ProcessAbility()
    {
        if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
        {
            base.ProcessAbility();

            HandleForwardMovement();
            CalculateDistance();
        }
    }

    protected virtual void HandleForwardMovement()
    {
        if (AbilityPermitted)
        {

            // if (!_justGotGrounded)
            //   return;

            float m_DesiredForwardSpeed = m_forwardMovementInput * forwardMovementSpeed;

            if (moveForwardWhileJumping)
            {
                if (_movement.CurrentState == CharacterStates.MovementStates.Jumping || _movement.CurrentState == CharacterStates.MovementStates.Falling)
                {
                    StopAbilityUsedSfx();
                    m_DesiredForwardSpeed = forwardMovementSpeedWhileJumping;
                }
            }

            float acceleration = m_forwardMovementInput > 0 ? k_GroundAcceleration : k_GroundDeceleration;
            MovementSpeed = Mathf.MoveTowards(MovementSpeed, m_DesiredForwardSpeed, acceleration * Time.deltaTime);

            if (_character._state.JustGotGrounded){
                MovementSpeed = 0f;
                m_DesiredForwardSpeed = 0f;
            }

            if ((_character._state.IsGrounded && MovementSpeed != 0) || (_movement.CurrentState == CharacterStates.MovementStates.Slipping && m_forwardMovementInput > 0))
            {
                if (_character.Friction < 1 && _character.Friction > 0)
                {
                    if (_movement.CurrentState != CharacterStates.MovementStates.WalkingOnSlipery)
                    {
                        PlayAbilityUsedSfx();
                        _movement.ChangeState(CharacterStates.MovementStates.WalkingOnSlipery);
                    }
                }
                else
                {
                    if (_movement.CurrentState != CharacterStates.MovementStates.Running)
                    {
                        PlayAbilityUsedSfx();
                        _movement.ChangeState(CharacterStates.MovementStates.Running);
                    }
                }
            }

            if ((_movement.CurrentState == CharacterStates.MovementStates.Running || _movement.CurrentState == CharacterStates.MovementStates.WalkingOnSlipery) && m_forwardMovementInput == 0 && _onSliperryPlatform)
            {
                StopAbilityUsedSfx();
                _movement.ChangeState(CharacterStates.MovementStates.Slipping);
            }

            MovementSpeed = HandleFriction(MovementSpeed);
            _character.SetForwardMovement(MovementSpeed);

            // if we're walking and not moving anymore, we go back to the Idle state
            if ((_movement.CurrentState == CharacterStates.MovementStates.Running || _movement.CurrentState == CharacterStates.MovementStates.WalkingOnSlipery)
                && (MovementSpeed == 0))
            {
                StopAbilityUsedSfx();
                _movement.ChangeState(CharacterStates.MovementStates.StopRunning);
            }

            else if (_movement.CurrentState == CharacterStates.MovementStates.StopRunning)
            {
                StopAbilityUsedSfx();
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
            }

            else if (_movement.CurrentState == CharacterStates.MovementStates.Slipping && (MovementSpeed == 0))
            {
                StopAbilityUsedSfx();
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
            }
        }
    }

    protected virtual void CalculateDistance()
    {
        if (_character._state.JustGotGrounded && !_justGotGrounded)
        {
            _initialZPosition = (int)transform.position.z;
            _justGotGrounded = true;
            tracker.distanceCovered = 0;
        }
        _distanceCovered = Mathf.Abs((int)transform.position.z - _initialZPosition);
        tracker.distanceCovered = _distanceCovered;
    }

    protected virtual float HandleFriction(float force)
    {
        _onSliperryPlatform = false;
        // if we have a friction above 1 (mud, water, stuff like that), we divide our speed by that friction
        if (_character.Friction > 1)
        {
            force = force / _character.Friction;
        }

        // if we have a low friction (ice, marbles...) we lerp the speed accordingly
        if (_character.Friction < 1 && _character.Friction > 0)
        {
            force = Mathf.Lerp(_character.Speed.z, force, Time.deltaTime * _character.Friction * 20);
            _onSliperryPlatform = true;
        }

        return force;
    }

    protected override void OnGotMove(Move currentMove)
    {
        if (!continousForwardMovement)
        {
            if (currentMove == Move.Running)
                m_forwardMovementInput = 1;
            else if (currentMove == Move.StopRunning)
                m_forwardMovementInput = 0;
        }
    }

    /// <summary>
    /// Adds required animator parameters to the animator parameters list if they exist
    /// </summary>
    protected override void InitializeAnimatorParameters()
    {
        RegisterAnimatorParameter("Running", AnimatorControllerParameterType.Bool);
        RegisterAnimatorParameter("StopRunning", AnimatorControllerParameterType.Bool);
        RegisterAnimatorParameter("Slipping", AnimatorControllerParameterType.Bool);
    }

    public override void Reset()
    {
        base.Reset();

        StopAbilityUsedSfx();
        MovementSpeed = 0;
        m_forwardMovementInput = 0;
    }

    private void OnDestroy()
    {
        Debug.Log("forward on destroy");
        Reset();
    }

    /// <summary>
    /// Sends the current speed and the current value of the Walking state to the animator
    /// </summary>
    public override void UpdateAnimator()
    {
        GSGAnimator.UpdateAnimatorBool(_animator, "Running", (_condition.CurrentState == CharacterStates.CharacterConditions.Normal &&
            (_movement.CurrentState == CharacterStates.MovementStates.Running || _movement.CurrentState == CharacterStates.MovementStates.WalkingOnSlipery)), _character._animatorParameters);

        GSGAnimator.UpdateAnimatorBool(_animator, "StopRunning", (_condition.CurrentState == CharacterStates.CharacterConditions.Normal &&
            _movement.CurrentState == CharacterStates.MovementStates.StopRunning), _character._animatorParameters);

        GSGAnimator.UpdateAnimatorBool(_animator, "Slipping", (_condition.CurrentState == CharacterStates.CharacterConditions.Normal &&
            _movement.CurrentState == CharacterStates.MovementStates.Slipping), _character._animatorParameters);
    }
}
