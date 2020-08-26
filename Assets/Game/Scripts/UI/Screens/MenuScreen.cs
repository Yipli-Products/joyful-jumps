using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Firebase.Database;
using System.Threading.Tasks;

public class MenuScreen : MonoBehaviour
{
    public TextMeshProUGUI levelIndex;

    public TextMeshProUGUI playerName;
    public TextMeshProUGUI coinsEarned;

    public PlayerGameData playerGameData;

    private void OnEnable()
    {
        ResetGamePopup.OnGameReset += UpdateLevel;
    }

    private void OnDisable()
    {
        ResetGamePopup.OnGameReset -= UpdateLevel;
    }

    private async void Start()
    {
        if (YipliHelper.checkInternetConnection())
        {
            PlayerSession.Instance.LoadingScreenSetActive(true);
            PlayerGameData tempData = await GameDataHandler.GetPlayerData();
            print("Task recieved");
            playerGameData.SetTotalScore(tempData.GetTotalScore());
            playerGameData.SetCurrentLevel(tempData.GetCurrentLevel());
            PlayerSession.Instance.LoadingScreenSetActive(false);
        }
        else
        {
            playerGameData.SetTotalScore(0);
            playerGameData.SetCurrentLevel(0);
        }
        
        levelIndex.text = "" + (playerGameData.GetCurrentLevel() + 1 );
        coinsEarned.text = string.Format("Coins : {0}", playerGameData.GetTotalScore());
        playerName.text = PlayerSession.Instance.GetCurrentPlayer();
    }

    private void SetLevelData()
    {
        levelIndex.text = "" + (playerGameData.GetCurrentLevel() + 1 );
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
        YipliHelper.GoToYipli();
    }

    public void ClickOnSelectAvatar()
    {
        UiManager.Instance.LoadUI(UiManager.SCREEN.CharacterSelectionScreen);
    }

    public void ClickOnNextLevel()
    {
        if (YipliHelper.GetBleConnectionStatus().Equals("connected", StringComparison.OrdinalIgnoreCase))
        {
            if (playerGameData.GetCurrentLevel() == 0)
                LoadingManager.Instance.LoadScreen(Constant.DREAM_SCENE_NAME);
            else
                LoadingManager.Instance.LoadScreen(Constant.GAME_SCENE_NAME);

            PlayerData.FootStep = 0;
            PlayerData.FallCount = 0;
            PlayerData.JumpStep = 0;
            PlayerData.FallCount = 0;
        }
    }

    void UpdateLevel()
    {
        SetLevelData();
    }
}
