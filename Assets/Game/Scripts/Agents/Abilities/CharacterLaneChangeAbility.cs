using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GodSpeedGames.Tools;

public class CharacterLaneChangeAbility : CharacterAbility
{
    [Tooltip("How fast player will move while changing lanes.")]
    public float sideMovementSpeed = 20f;
    public float SideMovementSpeed { get; set; }                // How fast Player is currently going along the side ways.

    const float k_GroundAcceleration = 20f;

    protected float xDirection = 0;
    [Header("Debug")]
    [ReadOnly] public float _currentLane = 0;
    [ReadOnly] public float _targetLane = 0;
    [ReadOnly] public bool laneReached = true;

    const float k_roadWidth = 7f;

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        HandleSideMovement();
    }

    public override void Reset()
    {
        base.Reset();
        _currentLane = 0;
        _targetLane = 0;
        laneReached = true;
        xDirection = 0;
        SideMovementSpeed = 0f;
    }

    protected virtual void LateUpdate()
    {
        ValidateLaneMovement();
    }

    void HandleSideMovement()
    {
        if (AbilityPermitted)
        {
            // float inputFromHardware = xDirection;
            // float m_DesiredSideSpeed = inputFromHardware * sideMovementSpeed;
            // SideMovementSpeed = Mathf.MoveTowards(SideMovementSpeed, m_DesiredSideSpeed, k_GroundAcceleration * Time.deltaTime);
            // SideMovementSpeed = m_DesiredSideSpeed * Time.deltaTime;
            if (xDirection > 0)
            {
                if (_movement.CurrentState != CharacterStates.MovementStates.RightMove)
                    _movement.ChangeState(CharacterStates.MovementStates.RightMove);
            }
            else if (xDirection < 0)
            {
                if (_movement.CurrentState != CharacterStates.MovementStates.LeftMove)
                    _movement.ChangeState(CharacterStates.MovementStates.LeftMove);
            }

            _character.SetHorizontalMovement(sideMovementSpeed * xDirection);
        }
    }

    void ValidateLaneMovement()
    {
        if (AbilityPermitted)
        {
            Vector3 position = transform.position;

            if (!laneReached)
            {
                float sideOffset = (Mathf.Abs(_targetLane - transform.position.x));
                if (sideOffset <= 0.3f)
                {

                    position.x = _currentLane = _targetLane;
                    transform.position = position;
                    laneReached = true;
                    xDirection = 0;
                    //Debug.Log(string.Format("f2 : current lane {0} : target lane {1}", _currentLane, _targetLane));
                }
                else
                {
                    if ((int)transform.position.x < -k_roadWidth)
                    {
                        position.x = _currentLane = _targetLane = -k_roadWidth;
                        transform.position = position;
                        laneReached = true;
                        xDirection = 0;
                        // Debug.Log(string.Format("f2 : current lane {0} : target lane {1}", _currentLane, _targetLane));
                    }
                    else if ((int)transform.position.x > k_roadWidth)
                    {
                        position.x = _currentLane = _targetLane = k_roadWidth;
                        transform.position = position;
                        laneReached = true;
                        xDirection = 0;
                        //Debug.Log(string.Format("f2 : current lane {0} : target lane {1}", _currentLane, _targetLane));

                    }
                }
            }
            else
            {
                if (position.x < 0 && (int)position.x < -k_roadWidth)
                {
                    position.x = -k_roadWidth;
                    transform.position = position;
                    _currentLane = -k_roadWidth;
                    //Debug.Log(string.Format("f2 : current lane {0} : target lane {1}", _currentLane, _targetLane));
                }
                else if (position.x > 0 && (int)position.x > k_roadWidth)
                {
                    position.x = k_roadWidth;
                    transform.position = position;
                    _currentLane = k_roadWidth;
                    // Debug.Log("Moving to position " + position.x);
                    //Debug.Log(string.Format("f2 : current lane {0} : target lane {1}", _currentLane, _targetLane));
                }
                else
                {
                    position.x = _currentLane;
                    transform.position = position;
                }
            }

            if (laneReached &&
                (_movement.CurrentState == CharacterStates.MovementStates.RightMove || _movement.CurrentState == CharacterStates.MovementStates.LeftMove)) {
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
            }
        }
    }

    /// <summary>
    /// Adds required animator parameters to the animator parameters list if they exist
    /// </summary>
    protected override void InitializeAnimatorParameters()
    {
        // RegisterAnimatorParameter("Speed", AnimatorControllerParameterType.Float);
        // RegisterAnimatorParameter("Walking", AnimatorControllerParameterType.Bool);
    }

    /// <summary>
    /// Sends the current speed and the current value of the Walking state to the animator
    /// </summary>
    public override void UpdateAnimator()
    {
        // GSGAnimator.UpdateAnimatorFloat(_animator, "Speed", Mathf.Abs(_normalizedHorizontalSpeed), _character._animatorParameters);
        //GSGAnimator.UpdateAnimatorBool(_animator, "Walking", (_movement.CurrentState == CharacterStates.MovementStates.Walking), _character._animatorParameters);
    }

    protected override void OnGotMove(Move currentMove)
    {
        switch (currentMove)
        {
            case Move.Left:
                if (_currentLane > -k_roadWidth)
                {
                    if (laneReached)
                    {
                        _targetLane = (_currentLane - k_roadWidth);
                        xDirection = -1;
                    }
                    // else
                    //_targetLane = _currentLane;

                    //Debug.Log(string.Format("mobing left : current lane {0} : target lane {1}", _currentLane, _targetLane));

                    // xDirection = -1;
                    laneReached = false;
                }

                break;

            case Move.Right:

                if (_currentLane < k_roadWidth)
                {
                    if (laneReached)
                    {
                        _targetLane = (_currentLane + k_roadWidth);
                        xDirection = 1;
                    }
                    //else
                    //    _targetLane = _currentLane;

                    //Debug.Log(string.Format("mobing right : current lane {0} : target lane {1}", _currentLane, _targetLane));

                    // xDirection = 1;
                    laneReached = false;
                }

                break;
        }
    }
}
