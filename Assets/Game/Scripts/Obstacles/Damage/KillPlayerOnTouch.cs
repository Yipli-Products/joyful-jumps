using UnityEngine;
using System.Collections;
using GodSpeedGames.Tools;

public class KillPlayerOnTouch : MonoBehaviour
{

    public bool isFloor = false;

    private Collider _collider;

    private void OnEnable()
    {
        LevelManager.OnGameEvent += OnGameEvent;
    }

    private void OnDisable()
    {
        LevelManager.OnGameEvent -= OnGameEvent;
    }

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    protected virtual void OnTriggerEnter(Collider collider)
    {
       
        Character character = collider.GetComponent<Character>();

        if (character == null)
        {
            return;
        }

        if (isFloor)
            _collider.enabled = false;

        LevelManager.Instance.KillPlayer(character, isFloor);
    }


    private void OnCollisionEnter(Collision collision)
    {
        Character character = collision.gameObject.GetComponent<Character>();

        if (character == null)
        {
            return;
        }

        if (isFloor)
            _collider.enabled = false;

        LevelManager.Instance.KillPlayer(character, isFloor);
    }

    void OnGameEvent(GSGGameEvent _event)
    {
        if (_event == GSGGameEvent.PlayerRespawn && _collider)
            _collider.enabled = true;
    }
}
