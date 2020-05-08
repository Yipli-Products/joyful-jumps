using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUpdate : MonoBehaviour
{
    [SerializeField] Text textUpdate;
    private Animator _anim;

    int counter = 0;
    void Start()
    {
        counter = 0;
        _anim = GetComponent<Animator>();

        if (_anim)
            LevelManager.OnGameEvent += GameEvent;
    }

    private void OnEnable()
    {
        InputController.OnSetMove += OnSetMove;
    }

    private void OnDisable()
    {
        InputController.OnSetMove -= OnSetMove;

        if (_anim)
            LevelManager.OnGameEvent -= GameEvent;
    }

    void OnSetMove(Move currentMove)
    {
        if (currentMove != Move.None)
        {
            if (textUpdate)
            {
                counter++;
                textUpdate.text = string.Format("{0} : {1}", counter, currentMove.ToString());
            }

            if (_anim)
            {
                _anim.SetBool("running", (currentMove == Move.Running));
                _anim.SetBool("stop_running", (currentMove == Move.StopRunning));
                _anim.SetBool("jumping", (currentMove == Move.Jump));
            }
        }
    }

    void GameEvent(GSGGameEvent _event)
    {
        _anim.SetBool("running", false);
        _anim.SetBool("stop_running", true);
        _anim.SetBool("jumping", false);
    }
}
