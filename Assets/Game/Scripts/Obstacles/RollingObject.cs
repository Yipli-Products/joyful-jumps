using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GodSpeedGames.Tools;

public class RollingObject : MonoBehaviour, IObstacle
{
    [Tooltip("If it activates rolling when player comes near to it, then set it true.")]
    public bool requireActivisionZone = false;

    [Condition("requireActivisionZone", true)]
    public Vector3 center = Vector3.zero;

    [Condition("requireActivisionZone", true)]
    public Vector3 size = new Vector3(1, 1, 1);

    [Tooltip("Player's Layer")]
    public LayerMask playerLayer;
    [Tooltip("Ground's Layer")]
    public LayerMask groundLayer;

    [Tooltip("Continous sound clip")]
    public GSGMusic soundSFX;

    [Header("Movement Speed")]
    public float speed;

    public bool randomness = false;

    [Condition("randomness", true)]
    public float minSpeed = 2f;

    [Condition("randomness", true)]
    public float maxSpeed = 3f;

    [Tooltip("Freez delay after spawning")]
    public float freezDelay = 2f;

    protected Rigidbody _rigidbody;
    protected Collider _collider;
    protected bool _stopRotating = false;

    private float _freezDelta = 0f;

    protected virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        if (randomness)
            speed = Random.Range(minSpeed, maxSpeed);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _freezDelta += Time.deltaTime;

        if (_freezDelta >= freezDelay)
        {
            _rigidbody.isKinematic = false;
            _collider.isTrigger = false;

            if (requireActivisionZone)
            {
                Collider[] m_buffer = new Collider[2];
                int count = Physics.OverlapBoxNonAlloc(transform.position + center, size / 2, m_buffer, transform.rotation, playerLayer);
                if (count > 0)
                {
                    StartRotating();
                }
            }
            else
                StartRotating();
        }
    }

    void StartRotating()
    {
        if (_rigidbody & !_stopRotating)
            _rigidbody.angularVelocity = transform.right * speed;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (requireActivisionZone)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube(transform.position + center, size);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (playerLayer.Contains(collision.gameObject))
        {
            _stopRotating = true;
            if (_rigidbody)
            {
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.velocity = Vector3.zero;
            }
        }
        else if (groundLayer.Contains(collision.gameObject))
        {
            SoundManager.Instance.PlayMusic(soundSFX, transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (groundLayer.Contains(collision.gameObject))
        {
            SoundManager.Instance.PlayMusic(soundSFX, transform);
        }
    }


    public void ResetRolling()
    {
        _stopRotating = false;
        if (_rigidbody)
        {
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _collider.isTrigger = true;
            _freezDelta = 0f;
        }
    }
}

public interface IObstacle
{
    void ResetRolling();
}
