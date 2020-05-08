using GodSpeedGames.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public bool deletePlayerPrefs;

    private void Start()
    {
        if (deletePlayerPrefs)
            PlayerPrefs.DeleteAll();
    }

    public static string FormatTime(int secondLeft)
    {
        string formattedText = "";

        int seconds = secondLeft % 60;
        int min = secondLeft / 60;

        if (seconds > 9)
            formattedText = string.Format("{0}m : {1}s", min, seconds);
        else
            formattedText = string.Format("{0}m : 0{1}s", min, seconds);

        return formattedText;
    }

    public void CalculateRewardPoint(LevelDifficulty levelDifficulty, GameDataTracker tracker)
    {
        float multiplayer = 1f;

        if (levelDifficulty.difficulty == Difficulty.Easy)
            multiplayer = 1f;
        else if (levelDifficulty.difficulty == Difficulty.Medium)
            multiplayer = 1.25f;
        else if (levelDifficulty.difficulty == Difficulty.Hard)
            multiplayer = 1.5f;

        float secondSaved = (float)tracker.totalTime - (float)tracker.timeElapsed;

        int pointOnBeforeTime = (int)(secondSaved * 5f * multiplayer);
        pointOnBeforeTime += (PlayerData.FootStep * 10) + (PlayerData.JumpStep * 50) - (PlayerData.FallCount * 50);

        if (pointOnBeforeTime < 0)
            pointOnBeforeTime = 0;

        tracker.totalPointsEarned = pointOnBeforeTime;
    }
}
