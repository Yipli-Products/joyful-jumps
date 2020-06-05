using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GamePlayScreen : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI timerText;

    public TextMeshProUGUI current_level_text;
    public TextMeshProUGUI current_reward_text;

    public Animation startText;

    public GameObject showJumpText;

    public GameDataTracker tracker;

    public PlayerGameData playerGameData;

    private int _previousTime = -1;
    private int _previousDistance = -1;

    private void OnEnable()
    {
        LevelManager.OnGameEvent += LevelEnd;
        PlayerDetector.ShowJumpNotification += OnShowJumpNotification;
        InputController.OnSetMove += OnGotMove;

        showJumpText.SetActive(false);
    }

    private void OnDisable()
    {
        LevelManager.OnGameEvent -= LevelEnd;
        PlayerDetector.ShowJumpNotification -= OnShowJumpNotification;
        InputController.OnSetMove -= OnGotMove;
    }

    private void Start()
    {
        current_level_text.text = string.Format("Level : {0}", playerGameData.GetCurrentLevel() + 1);
        current_reward_text.text = string.Format("Coins : {0}", playerGameData.GetTotalScore());
        StartCoroutine(BluetoothCheck());
    }

    void LevelEnd(GSGGameEvent _event)
    {
        if (_event == GSGGameEvent.LevelOver)
        {
            UiManager.Instance.LoadPopup(UiManager.Popup.GameOverScreen);
        }
        else if (_event == GSGGameEvent.StartTimer)
        {
            startText.Play();
        }

        showJumpText.SetActive(false);
    }

    public void ClickOnPause()
    {
        UiManager.Instance.LoadPopup(UiManager.Popup.PauseScreen);
    }

    private void Update()
    {
        // if (_previousTime != tracker.timeElapsed)
        // {
        _previousTime = tracker.timeElapsed;
        timerText.text = GameManager.FormatTime(tracker.totalTime - _previousTime);
        //}

        if (_previousDistance != tracker.distanceCovered)
        {
            _previousDistance = tracker.distanceCovered;
            speedText.text = "" + _previousDistance;
        }
    }

    //Pause the game if Bluetooth isnt connected.
    private IEnumerator BluetoothCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (!PlayerSession.Instance.GetBleConnectionStatus().Equals("Connected", StringComparison.OrdinalIgnoreCase) && (Time.timeScale == 1f))
            {
                //Pause the game.
                Debug.Log("Pausing the game as the bluetooth isnt connected.");
                UiManager.Instance.LoadPopup(UiManager.Popup.PauseScreen);
            }
        }
    }

    void OnShowJumpNotification(bool show)
    {
        showJumpText.SetActive(show);
    }

    void OnGotMove(Move currentMove)
    {
        if (currentMove == Move.KeyPause)
            ClickOnPause();
    }
}
