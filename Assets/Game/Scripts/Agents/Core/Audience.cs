using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GodSpeedGames.Tools;

public class Audience : MonoBehaviour
{
    private Animator _animator;

    private void OnEnable()
    {
        LevelManager.OnGameEvent += OnGameEvent;
    }

    private void OnDisable()
    {
        LevelManager.OnGameEvent -= OnGameEvent;
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void OnGameEvent(GSGGameEvent _event) {
        if (_event == GSGGameEvent.LevelSuccess) {
            _animator.SetBool("Idle", false);
            _animator.SetTrigger("Success");
        }
    }
}
