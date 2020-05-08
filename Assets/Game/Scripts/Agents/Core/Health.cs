using GodSpeedGames.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static System.Action OnHit;
    public static System.Action OnDeath;
    public static System.Action OnRevive;

    [ReadOnly]
    public int CurrentHealth;
    /// If this is true, this object can't take damage
    [ReadOnly]
    public bool Invulnerable = false;

    [Tooltip("the initial amount of health of the object")]
    public int InitialHealth = 10;
    [Tooltip("the maximum amount of health of the object")]
    public int MaximumHealth = 10;

    [Header("Damage")]
    [Tooltip("the effect that will be instantiated everytime the character touches the ground")]
    public GameObject DamageEffect;

    [Tooltip("the sound to play when the player gets hit")]
    public GSGMusic DamageSfx;
    [Tooltip("the sound to play when the player won the level")]
    public GSGMusic WonSfx;

    [Header("Death")]
    [Tooltip("the effect to instantiate when the object gets destroyed")]
    public GameObject DeathEffect;
    public GameObject OopsEffect;
    [Tooltip("the time (in seconds) before the character is destroyed or disabled")]
    public float DelayBeforeDestruction = 0f;
    [Tooltip("if this is true, collisions will be turned off when the character dies")]
    public bool CollisionsOffOnDeath = true;
    [Tooltip("if true, the handheld device will vibrate when the object dies")]
    public bool VibrateOnDeath;
    [Tooltip("if this is set to false, the character will respawn at the location of its death, otherwise it'll be moved to its initial position (when the scene started)")]
    public bool RespawnAtInitialLocation = false;

    protected Vector3 _initialPosition;
    protected Character _character;
    //protected Collider2D _collider2D;
    protected bool _initialized = false;
    // protected AutoRespawn _autoRespawn;
    protected Animator _animator;
    protected Camera _cam;

    protected virtual void Start()
    {
        Initialization();
    }

    /// <summary>
    /// Grabs useful components, enables damage and gets the inital color
    /// </summary>
    protected virtual void Initialization()
    {
        _character = GetComponent<Character>();

        // we grab our animator
        /*if (_character != null)
        {
            if (_character.CharacterAnimator != null)
            {
                _animator = _character.CharacterAnimator;
            }
            else
            {
                _animator = GetComponent<Animator>();
            }
        }
        else
        {
            _animator = GetComponent<Animator>();
        }*/

        _animator = _character._animator;

        if (_animator != null)
        {
            _animator.logWarnings = false;
        }


        _initialPosition = transform.position;
        _initialized = true;
        CurrentHealth = InitialHealth;
        DamageEnabled();

        _cam = Camera.main;
    }

    /// <summary>
    /// Called when the object takes damage
    /// </summary>
    /// <param name="damage">The amount of health points that will get lost.</param>
    /// <param name="instigator">The object that caused the damage.</param>
    /// <param name="flickerDuration">The time (in seconds) the object should flicker after taking the damage.</param>
    /// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
    public virtual void Damage(int damage, GameObject instigator, float flickerDuration, float invincibilityDuration)
    {
        // if the object is invulnerable, we do nothing and exit
        if (Invulnerable)
        {
            return;
        }

        // if we're already below zero, we do nothing and exit
        if ((CurrentHealth <= 0) && (InitialHealth != 0))
        {
            return;
        }

        // we decrease the character's health by the damage
        float previousHealth = CurrentHealth;
        CurrentHealth -= damage;

        OnHit?.Invoke();

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        // we prevent the character from colliding with Projectiles, Player and Enemies
        if (invincibilityDuration > 0)
        {
            DamageDisabled();
            StartCoroutine(DamageEnabled(invincibilityDuration));
        }

        // we trigger a damage taken event
        // GSGDamageTakenEvent.Trigger(_character, instigator, CurrentHealth, damage, previousHealth);

        if (_animator != null)
        {
            //_animator.SetTrigger("Damage");
        }

        // we play the sound the player makes when it gets hit
        PlayHitSfx();

        // When the character takes damage, we create an auto destroy hurt particle system
        if (DamageEffect != null)
        {
            Instantiate(DamageEffect, transform.position, transform.rotation);
        }

        // if health has reached zero
        if (CurrentHealth <= 0)
        {
            // we set its health to zero (useful for the healthbar)
            CurrentHealth = 0;
            if (_character != null)
            {
                LevelManager.Instance.KillPlayer(_character, false);
                return;
            }
        }
    }

    /// <summary>
    /// Kills the character, vibrates the device, instantiates death effects, handles points, etc
    /// </summary>
    public virtual void Kill(bool floorKill)
    {
        // we make our handheld device vibrate
        if (VibrateOnDeath)
        {
#if UNITY_ANDROID || UNITY_IPHONE
            Handheld.Vibrate();
#endif
        }

        // we prevent further damage
        DamageDisabled();

        // instantiates the destroy effect
        if (DeathEffect != null)
        {
            //GameObject instantiatedEffect = (GameObject)Instantiate(DeathEffect, transform.position + Vector3.up, transform.rotation);
            //instantiatedEffect.transform.localScale = transform.localScale;
            Vector3 position = _cam.transform.position + new Vector3(0, 0, 7);
            position.y = transform.position.y + 1;
            GameObject instantiatedEffect = (GameObject)Instantiate(floorKill ? OopsEffect : DeathEffect, position, Quaternion.identity);
        }

        if (_animator != null && !floorKill)
        {
            _animator.SetTrigger("Death");
        }

        // we play the sound the player makes when it gets hit
        if (!floorKill)
            PlayHitSfx();

        OnDeath?.Invoke();

        // if we have a controller, removes collisions, restores parameters for a potential respawn, and applies a death force
        if (_character != null)
        {
            // we make it ignore the collisions from now on
            if (CollisionsOffOnDeath)
            {
                _character.CollisionsOff(floorKill);
            }

            // we reset our parameters
            _character.ResetParameters();

            if(floorKill)
                _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Falling);
            else
                _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Dead);

            _character.Reset();

        }
    }

    public virtual void WinLevel()
    {
        // we prevent further damage
        DamageDisabled();

        if (_animator != null)
        {
            _animator.SetTrigger("Success");
        }

        SoundManager.Instance.PlayMusic(WonSfx, transform);

        // if we have a controller, removes collisions, restores parameters for a potential respawn, and applies a death force
        if (_character != null)
        {
            // we reset our parameters
            _character.ResetParameters();
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Won);
            _character.Reset();
        }
    }

    /// <summary>
    /// Revive this object.
    /// </summary>
    public virtual void Revive()
    {
        if (!_initialized)
        {
            return;
        }

        if (RespawnAtInitialLocation)
        {
            transform.position = _initialPosition;
        }

        Initialization();
        OnRevive?.Invoke();
    }

    public virtual void ResetHealthToMaxHealth()
    {
        CurrentHealth = MaximumHealth;
    }

    /// <summary>
    /// makes the character able to take damage again after the specified delay
    /// </summary>
    /// <returns>The layer collision.</returns>
    public virtual IEnumerator DamageEnabled(float delay)
    {
        yield return new WaitForSeconds(delay);
        Invulnerable = false;
    }

    /// <summary>
    /// Allows the character to take damage
    /// </summary>
    public virtual void DamageEnabled()
    {
        Invulnerable = false;
    }

    /// <summary>
    /// Prevents the character from taking any damage
    /// </summary>
    public virtual void DamageDisabled()
    {
        Invulnerable = true;
    }

    /// <summary>
    /// Plays a sound when the character is hit
    /// </summary>
    protected virtual void PlayHitSfx()
    {
        //if (DamageSfx != null)
        {
            SoundManager.Instance.PlayMusic(DamageSfx, transform);
        }
    }
}
