using GodSpeedGames.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterAbility : MonoBehaviour
{
    /// the sound fx to play when the ability starts
    public GSGMusic AbilityStartSfx;
    /// the sound fx to play while the ability is running
    public GSGMusic AbilityInProgressSfx;
    /// the sound fx to play when the ability stops
    public GSGMusic AbilityStopSfx;

    /// if true, this ability can perform as usual, if not, it'll be ignored. You can use this to unlock abilities over time for example
    public bool AbilityPermitted = true;

    public bool AbilityInitialized { get { return _abilityInitialized; } }

    protected Character _character;
    protected Animator _animator;
    protected CharacterStates _state;
    protected CharacterController _controller;
    protected GSGStateMachine<CharacterStates.MovementStates> _movement;
    protected GSGStateMachine<CharacterStates.CharacterConditions> _condition;
    protected AudioSource _abilityInProgressSfx;
    protected bool _abilityInitialized = false;

    protected float _verticalInput;
    protected float _horizontalInput;

    protected virtual void Start()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        _character = GetComponent<Character>();
        _controller = GetComponent<CharacterController>();
        BindAnimator();
        _state = _character.CharacterState;
        _movement = _character.MovementState;
        _condition = _character.ConditionState;
        _abilityInitialized = true;
    }

    protected virtual void BindAnimator()
    {
        _animator = _character._animator;
        if (_animator != null)
        {
            InitializeAnimatorParameters();
        }
    }

    /// <summary>
    /// Adds required animator parameters to the animator parameters list if they exist
    /// </summary>
    protected virtual void InitializeAnimatorParameters()
    {

    }

    /// <summary>
    /// The first of the 3 passes you can have in your ability. Think of it as EarlyUpdate() if it existed
    /// </summary>
    public virtual void EarlyProcessAbility()
    {

    }

    /// <summary>
    /// The second of the 3 passes you can have in your ability. Think of it as Update()
    /// </summary>
    public virtual void ProcessAbility()
    {

    }

    /// <summary>
    /// The last of the 3 passes you can have in your ability. Think of it as LateUpdate()
    /// </summary>
    public virtual void LateProcessAbility()
    {

    }

    /// <summary>
    /// Override this to send parameters to the character's animator. This is called once per cycle, by the Character class, after Early, normal and Late process().
    /// </summary>
    public virtual void UpdateAnimator()
    {

    }

    /// <summary>
    /// Changes the status of the ability's permission
    /// </summary>
    /// <param name="abilityPermitted">If set to <c>true</c> ability permitted.</param>
    public virtual void PermitAbility(bool abilityPermitted)
    {
        AbilityPermitted = abilityPermitted;
    }

    /// <summary>
    /// Override this to specify what should happen in this ability when the character flips
    /// </summary>
    public virtual void Flip()
    {

    }

    /// <summary>
    /// Override this to reset this ability's parameters. It'll be automatically called when the character gets killed, in anticipation for its respawn.
    /// </summary>
    public virtual void Reset()
    {

    }

    /// <summary>
    /// Plays the ability start sound effect
    /// </summary>
    protected virtual void PlayAbilityStartSfx()
    {
        SoundManager.Instance.PlayMusic(AbilityStartSfx, transform);
    }

    /// <summary>
    /// Plays the ability used sound effect
    /// </summary>
    protected virtual void PlayAbilityUsedSfx()
    {
        /* if (AbilityInProgressSfx != null)
         {
             if (_abilityInProgressSfx == null)
             {
                 //_abilityInProgressSfx = SoundManager.Instance.PlaySound(AbilityInProgressSfx, transform.position, true);
             }
         }*/

        if (_abilityInProgressSfx == null)
        {
            _abilityInProgressSfx = SoundManager.Instance.PlayMusic(AbilityInProgressSfx, transform);
        }
    }

    /// <summary>
    /// Stops the ability used sound effect
    /// </summary>
    protected virtual void StopAbilityUsedSfx()
    {
        if (_abilityInProgressSfx != null)
        {
            //play sound
            _abilityInProgressSfx.Stop();
            Destroy(_abilityInProgressSfx.gameObject);
            _abilityInProgressSfx = null;
        }
    }

    /// <summary>
    /// Plays the ability stop sound effect
    /// </summary>
    protected virtual void PlayAbilityStopSfx()
    {
        // if (AbilityStopSfx != null)
        {
            //play sound
        }
    }

    /// <summary>
    /// Registers a new animator parameter to the list
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="parameterType">Parameter type.</param>
    protected virtual void RegisterAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (_animator == null)
        {
            return;
        }
        /*if (_animator.HasParameterOfType(parameterName, parameterType))
        {
            _character._animatorParameters.Add(parameterName);
        }*/

        GSGAnimator.AddAnimatorParamaterIfExists(_animator, parameterName, parameterType, _character._animatorParameters);
    }

    /// <summary>
    /// Override this to describe what should happen to this ability when the character respawns
    /// </summary>
    protected virtual void OnRespawn()
    {
    }

    /// <summary>
    /// Override this to describe what should happen to this ability when the character respawns
    /// </summary>
    protected virtual void OnDeath()
    {
        StopAbilityUsedSfx();
    }

    /// <summary>
    /// Override this to describe what should happen to this ability when the character takes a hit
    /// </summary>
    protected virtual void OnHit()
    {

    }

    /// <summary>
    /// On enable, we bind our respawn delegate
    /// </summary>
    protected virtual void OnEnable()
    {
        InputController.OnSetMove += OnGotMove;
    }

    /// <summary>
    /// On disable, we unbind our respawn delegate
    /// </summary>
    protected virtual void OnDisable()
    {
        InputController.OnSetMove -= OnGotMove;
    }

    protected virtual void OnGotMove(Move currentMove)
    {

    }


}
