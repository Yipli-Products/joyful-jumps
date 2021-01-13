using GodSpeedGames.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Character : MonoBehaviour
{
    [System.Serializable]
    public struct State
    {
        public bool IsGrounded;
        public bool WasGroundedLastFrame;
        public bool JustGotGrounded;
    }

    public GameDataTracker tracker;
    public CharacterScriptable characterScriptable;

    public State _state;
    public Animator CharacterAnimator;
    public CharacterStates CharacterState { get; protected set; }

    public float gravity = 20f;

    public GSGStateMachine<CharacterStates.MovementStates> MovementState;
    public GSGStateMachine<CharacterStates.CharacterConditions> ConditionState;

    public Animator _animator { get; protected set; }
    public List<string> _animatorParameters { get; set; }

    protected float _friction;

    protected CharacterAbility[] _characterAbilities;
    protected CharacterController _controller;
    protected Health _health;
    protected Collider _collider;
    protected Vector3 _movement;

    public float Friction { get { return _friction; } }

    [ReadOnly] public Vector3 _speed;

    protected virtual void Awake()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        // we initialize our state machines
        MovementState = new GSGStateMachine<CharacterStates.MovementStates>(gameObject, true);
        ConditionState = new GSGStateMachine<CharacterStates.CharacterConditions>(gameObject, true);

        _characterAbilities = GetComponents<CharacterAbility>();
        _controller = GetComponent<CharacterController>();
        _collider = GetComponent<Collider>();
        _health = GetComponent<Health>();

        // we get the current input manager
        //  GetInputManager();
        CharacterState = new CharacterStates();
        ConditionState.ChangeState(CharacterStates.CharacterConditions.Frozen);
        AssignAnimator();
    }

    public virtual void AssignAnimator()
    {
        /*if (CharacterAnimator != null)
        {
            _animator = CharacterAnimator;
        }
        else
        {
            _animator = GetComponent<Animator>();
        }*/

        GameObject playerAvatar = Instantiate<GameObject>(characterScriptable.characterData[PlayerData.SelectedAvatarIndex].playerPrefab);
        playerAvatar.transform.position = Vector3.zero;
        playerAvatar.transform.localRotation = Quaternion.identity;
        playerAvatar.transform.SetParent(transform);

        _animator = playerAvatar.GetComponent<Animator>();

        if (_animator != null)
        {
            InitializeAnimatorParameters();
        }
    }

    public virtual void SetForwardMovement(float Movement)
    {
        _movement.z = Movement;
    }

    public virtual void SetHorizontalMovement(float Movement)
    {
        _movement.x = Movement;
    }

    public virtual void SetVerticalMovement(float Movement)
    {
        _movement.y = Movement;
    }

    public virtual void Move(Vector3 _deltaMovement)
    {
        _controller.Move(_deltaMovement);
    }

    public virtual void CollisionsOff(bool floorKill)
    {
        _collider.enabled = false;
        if (!floorKill)
            _controller.enabled = false;
    }

    public virtual void CollisionsOn()
    {
        _collider.enabled = true;
        _controller.enabled = true;
    }

    public virtual void ResetParameters()
    {
        _friction = 0f;
    }

    public virtual void AddFriction(float friction)
    {
        _friction = friction;
    }

    public virtual void PlayStartDemoCutsceneAnimation()
    {
        _animator.SetTrigger("warmup");
    }

    /// <summary>
    /// Called when the Character dies. 
    /// Calls every abilities' Reset() method, so you can restore settings to their original value if needed
    /// </summary>
    public virtual void Reset()
    {
        if (_characterAbilities == null)
        {
            return;
        }
        if (_characterAbilities.Length == 0)
        {
            return;
        }
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled)
            {
                ability.Reset();
            }
        }
    }

    /// <summary>
    /// Makes the player respawn at the location passed in parameters
    /// </summary>
    /// <param name="spawnPoint">The location of the respawn.</param>
    public virtual void RespawnAt(Transform spawnPoint)
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("Spawn : your Character's gameobject is inactive");
            return;
        }

        transform.position = spawnPoint.position;

        // we make it handle collisions again
        CollisionsOn();
        ResetParameters();

        if (_health != null)
        {
            _health.ResetHealthToMaxHealth();
            _health.Revive();
        }

        // we raise it from the dead (if it was dead)
        ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
    }

    protected virtual void InitializeAnimatorParameters()
    {
        if (_animator == null) { return; }

        _animatorParameters = new List<string>();

        GSGAnimator.AddAnimatorParamaterIfExists(_animator, "Idle", AnimatorControllerParameterType.Bool, _animatorParameters);
    }

    protected virtual void Update()
    {
        EveryFrame();
    }

    /// <summary>
    /// We do this every frame. This is separate from Update for more flexibility.
    /// </summary>
    protected virtual void EveryFrame()
    {

        _speed = _controller.velocity;

        HandleCharacterStatus();

        // we process our abilities
        EarlyProcessAbilities();
        ProcessAbilities();
        LateProcessAbilities();

        UpdateMovement();

        UpdateAnimators();

        _speed = _controller.velocity;

        Vector3 forwardSpeed = _speed;
        forwardSpeed.x = 0;
        forwardSpeed.y = 0;
        // tracker.distanceCovered = (int)(forwardSpeed.magnitude * 3.6f);
    }

    /// <summary>
    /// Calls all registered abilities' Early Process methods
    /// </summary>
    protected virtual void EarlyProcessAbilities()
    {
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.EarlyProcessAbility();
            }
        }
    }

    /// <summary>
    /// Calls all registered abilities' Process methods
    /// </summary>
    protected virtual void ProcessAbilities()
    {
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.ProcessAbility();
            }
        }
    }

    /// <summary>
    /// Calls all registered abilities' Late Process methods
    /// </summary>
    protected virtual void LateProcessAbilities()
    {
        foreach (CharacterAbility ability in _characterAbilities)
        {
            if (ability.enabled && ability.AbilityInitialized)
            {
                ability.LateProcessAbility();
            }
        }
    }

    protected virtual void UpdateAnimators()
    {
        if (_animator != null)
        {
            GSGAnimator.UpdateAnimatorBool(_animator, "Idle", (ConditionState.CurrentState == CharacterStates.CharacterConditions.Normal &&
                MovementState.CurrentState == CharacterStates.MovementStates.Idle), _animatorParameters);

            foreach (CharacterAbility ability in _characterAbilities)
            {
                if (ability.enabled && ability.AbilityInitialized)
                {
                    ability.UpdateAnimator();
                }
            }
        }
    }

    /// <summary>
    /// Handles the character status.
    /// </summary>
    protected virtual void HandleCharacterStatus()
    {
        // if the character is dead, we prevent it from moving horizontally		
        if (ConditionState.CurrentState == CharacterStates.CharacterConditions.Falling)
        {
            _movement.z = 0;
        }

        // if the character is frozen, we prevent it from moving
        if (ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen)
        {
            //_controller.GravityActive(false);
            // _controller.SetForce(Vector2.zero);
        }

        _state.WasGroundedLastFrame = _state.IsGrounded;
    }

    protected virtual void UpdateMovement()
    {
        if (ConditionState.CurrentState == CharacterStates.CharacterConditions.Normal ||
            ConditionState.CurrentState == CharacterStates.CharacterConditions.Won ||
            ConditionState.CurrentState == CharacterStates.CharacterConditions.Falling ||
            ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
        {
            if (ConditionState.CurrentState == CharacterStates.CharacterConditions.Won || ConditionState.CurrentState == CharacterStates.CharacterConditions.Falling || ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
                _movement.z = 0;

            _controller.Move(_movement * Time.deltaTime);
            _state.IsGrounded = _controller.isGrounded;

            if (!_state.WasGroundedLastFrame && _state.IsGrounded)
                _state.JustGotGrounded = true;
            else
                _state.JustGotGrounded = false;
        }
    }

    public Vector3 Speed
    {
        get
        {
            return _speed;
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(Character))]
[CanEditMultipleObjects]
public class CharacterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Character character = (Character)target;


        // adds movement and condition states
        if (character.CharacterState != null)
        {
            EditorGUILayout.LabelField("Movement State", character.MovementState.CurrentState.ToString());
            EditorGUILayout.LabelField("Condition State", character.ConditionState.CurrentState.ToString());
        }

        DrawDefaultInspector();
    }
}
#endif


