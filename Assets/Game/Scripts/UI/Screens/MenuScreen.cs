using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuScreen : MonoBehaviour
{
    public TextMeshProUGUI levelIndex;

    public TextMeshProUGUI playerName;
    public TextMeshProUGUI coinsEarned;

    private void OnEnable()
    {
        ResetGamePopup.OnGameReset += UpdateLevel;
    }

    private void OnDisable()
    {
        ResetGamePopup.OnGameReset -= UpdateLevel;
    }

    private void Start()
    {
        levelIndex.text = "" + ( PlayerData.CurrentLevel + 1 );
        coinsEarned.text = string.Format("Coins : {0}", PlayerData.RewardCoin);
        if (PlayerSession.Instance)
            playerName.text = PlayerSession.Instance.GetCurrentPlayer();
    }

    private void SetLevelData()
    {
        levelIndex.text = "" + ( PlayerData.CurrentLevel + 1 );
    }

    public void ClickOnSettings()
    {
        UiManager.Instance.LoadPopup(UiManager.Popup.SettingsScreen);
    }

    public void ClickOnChangeProfile()
    {
        PlayerSession.Instance.ChangePlayer();
    }

    public void GoToYipliApp()
    {
        PlayerSession.Instance.GoToYipli();
    }

    public void ClickOnSelectAvatar()
    {
        UiManager.Instance.LoadUI(UiManager.SCREEN.CharacterSelectionScreen);
    }

    public void ClickOnNextLevel()
    {
        if (PlayerData.CurrentLevel == 0)
            LoadingManager.Instance.LoadScreen(Constant.DREAM_SCENE_NAME);
        else
            LoadingManager.Instance.LoadScreen(Constant.GAME_SCENE_NAME);

        PlayerData.FootStep = 0;
        PlayerData.FallCount = 0;
        PlayerData.JumpStep = 0;
        PlayerData.FallCount = 0;
    }

    void UpdateLevel()
    {
        SetLevelData();
    }
}
