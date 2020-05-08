using UnityEngine;
using System.Collections;
using GodSpeedGames.Tools;

public class Pendulum : MonoBehaviour
{
    [System.Serializable]
    public enum Direction
    {
        Right,
        Left
    }

    [Header("Sound SFX")]
    public GSGMusic soundSFX;
    public float sfxPlayInterval;

    [Header("Physical Properties")]
    [Range(10, 89)]
    public float angle = 60.0f;

    public float speed = 1.5f;

    public bool randomness = false;

    [Condition("randomness", true)]
    public float minSpeed = 2f;

    [Condition("randomness", true)]
    public float maxSpeed = 3f;

    public Direction direction;

    Quaternion qStart, qEnd;
    private float startTime;
    private float _soundSFXDelta;
    private bool playSound = false;

    void Start()
    {
        qStart = Quaternion.AngleAxis((direction == Direction.Right) ? angle : -angle, transform.right) * transform.localRotation;
        qEnd = Quaternion.AngleAxis((direction == Direction.Right) ? -angle : angle, transform.right) * transform.localRotation;

        if (randomness)
            speed = Random.Range(minSpeed, maxSpeed);
    }
    void Update()
    {
        startTime += Time.deltaTime;
        transform.rotation = Quaternion.Lerp(qStart, qEnd, (Mathf.Sin(startTime * speed + Mathf.PI / 2) + 1.0f) / 2.0f);

        float _angle = Mathf.Abs(transform.rotation.eulerAngles.x);
        if (_angle > 180)
            _angle = 360 - _angle;

        if (_angle > (Mathf.Abs(angle) - 5)) {
            // in the region
            _soundSFXDelta = 0f;
            playSound = true;
        }

        _soundSFXDelta += Time.deltaTime;
        if (_soundSFXDelta >= sfxPlayInterval && playSound)
        {
            //_soundSFXDelta = 0f;
            playSound = false;
            SoundManager.Instance.PlayMusic(soundSFX, transform);
        }
    }
}