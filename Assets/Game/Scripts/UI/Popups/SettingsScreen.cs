using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsScreen : MonoBehaviour
{
    public TextMeshProUGUI _soundText;

    private void Start()
    {
        SetSoundText();
    }

    public void ClickOnSound()
    {
        PlayerData.Sound = !PlayerData.Sound;
        SetSoundText();
        if (!PlayerData.Sound)
            SoundManager.Instance.TurnOffMusic();
        else
            SoundManager.Instance.TurnOnMusic();
    }

    public void ClickOnResetGame()
    {
        UiManager.Instance.LoadPopup(UiManager.Popup.ResetGamePopup);
        ClickOnClose();
    }

    public void ClickOnClose()
    {
        Destroy(gameObject);
    }

    void SetSoundText()
    {
        if (PlayerData.Sound)
            _soundText.text = "Sound On";
        else
            _soundText.text = "Sound Off";
    }

    public void RetakeMatTutorial()
    {
        PlayerSession.Instance.RetakeMatControlsTutorial();
    }

    public void TroubleShootSystem()
    {
        PlayerSession.Instance.TroubleShootSystem();
    }
}
