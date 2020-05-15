using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerGameData", menuName = "Godspeed/PlayerGameData")]
public class PlayerGameData : ScriptableObject
{
    private string playerName;
    private int currentLevel;
    private int totalScore;

    public void SetPlayerName(string pName)
    {
        playerName = pName;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetCurrentLevel(int iNextLevel)
    {
        currentLevel = iNextLevel;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void SetTotalScore(int score)
    {
        totalScore = score;
    }

    public int GetTotalScore()
    {
        return totalScore;
    }
}
