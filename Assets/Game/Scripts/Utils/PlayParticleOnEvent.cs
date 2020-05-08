using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleOnEvent : MonoBehaviour
{
    public GSGGameEvent _event;
    public GameObject particle;

    private void OnEnable()
    {
        LevelManager.OnGameEvent += OnGameEvent;
    }

    private void OnDisable()
    {
        LevelManager.OnGameEvent -= OnGameEvent;
    }

    void OnGameEvent(GSGGameEvent _event)
    {
        if (this._event == _event)
        {
            Instantiate(particle, transform.position, transform.rotation);
        }
    }
}
