using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI totalTimeTaken;
    public TextMeshProUGUI coinTaken;
    public TextMeshProUGUI caloriesBurned;
    public TextMeshProUGUI FitnessPointsEarned;

    public TextMeshProUGUI titleHeading;

    public GameDataTracker tracker;
    public LevelData levelData;

    public PlayerGameData playerGameData;

    public string[] reportCardMessages = {"Well done...!!", "Awesome game !!", "Good Job !!", "Fantastic play !!", "Good going champ !!"};

    /*protected void Start()
    {
       // base.Start();
        Time.timeScale = .00000001f;

        if (PlayerData.CurrentLevel == -1)
        {
            titleHeading.text = "Tutorial Complete.";
        }
        else
            titleHeading.text = string.Format("Level {0} Complete", (PlayerData.CurrentLevel + 2));

        totalTimeTaken.text = GameManager.FormatTime(tracker.timeElapsed);

        int nextLevel = PlayerData.CurrentLevel + 1;
        if (nextLevel >= (levelData.levelInfo.Length - 1))
            nextLevel = levelData.levelInfo.Length - 1;
        PlayerData.CurrentLevel = nextLevel;
    }*/

    private void Start()
    {
        SetData();
    }

    public void SetData()
    {
        int currentLevel = playerGameData.GetCurrentLevel() ;
        float calories = YipliPlayerActivity.GetCaloriesBurned(PlayerSession.Instance.getPlayerActionCounts());
        float fitnessPoints = YipliPlayerActivity.GetFitnessPoints(PlayerSession.Instance.getPlayerActionCounts());

        //titleHeading.text = string.Format("Well Done. Level {0} Complete", (currentLevel));
        titleHeading.text = reportCardMessages[Random.Range(0,4)];

        totalTimeTaken.text = "Total Time : "+GameManager.FormatTime(tracker.timeElapsed);
        coinTaken.text = string.Format("Rewards Earned : {0}", tracker.totalPointsEarned);
        caloriesBurned.text = "Calories burned : " + calories;
        FitnessPointsEarned.text = "Fitness points : " + fitnessPoints;

        tracker.timeElapsed = 0;

        PlayerData.FootStep = 0;
        PlayerData.FallCount = 0;
        PlayerData.JumpStep = 0;
        PlayerData.FallCount = 0;
    }
}
