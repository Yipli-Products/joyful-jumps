using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGamePopup : MonoBehaviour
{
    public static System.Action OnGameReset;

    public PlayerGameData playerGameData;

    public void OnReset() {
        PlayerPrefs.DeleteAll();
        SoundManager.Instance.TurnOnMusic();

        Dictionary<string, object> gameData = new Dictionary<string, object>();

        gameData.Add("reward-points", "0");
        gameData.Add("current-level", "0");

        PlayerSession.Instance.UpdateStoreData(gameData);

        playerGameData.SetCurrentLevel(0);
        playerGameData.SetTotalScore(0);

        OnGameReset?.Invoke();
        Destroy(gameObject);
    }

    public void Cancel() {
        Destroy(gameObject);
    }
}
