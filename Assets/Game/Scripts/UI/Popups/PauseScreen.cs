using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{

    public Image soundIcon;
    public Sprite soundOn, soundOff;

    protected virtual void Start()
    {
        Time.timeScale = .00000001f;
        if (UnityFitmatBridge.Instance != null)
            UnityFitmatBridge.Instance.DisableGameInput();
        if (PlayerSession.Instance != null)
            PlayerSession.Instance.PauseSPSession();
        SetSoundImage();
    }

    public virtual void ResumeGame()
    {
        if (PlayerSession.Instance.GetBleConnectionStatus().Equals("Connected", StringComparison.OrdinalIgnoreCase))
        {
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.ResumeSPSession();
            Time.timeScale = 1;
            if (UnityFitmatBridge.Instance != null)
                UnityFitmatBridge.Instance.EnableGameInput();
            Destroy(gameObject);
        }
    }

    public virtual void RestartGame()
    {
        Time.timeScale = 1;

        LoadingManager.Instance.LoadScreen(Constant.GAME_SCENE_NAME);
        Destroy(gameObject);
    }

    public virtual void MainMenu()
    {
        Time.timeScale = 1;
        LoadingManager.Instance.LoadScreen(Constant.MENU_SCENE_NAME);
        Destroy(gameObject);
    }

    public virtual void ClickOnSound() {
        PlayerData.Sound = !PlayerData.Sound;
        SetSoundImage();
        if (!PlayerData.Sound)
            SoundManager.Instance.TurnOffMusic();
        else
            SoundManager.Instance.TurnOnMusic();
    }

    void SetSoundImage()
    {
        if (PlayerData.Sound)
            soundIcon.sprite = soundOn;
        else
            soundIcon.sprite = soundOff;
    }
}
