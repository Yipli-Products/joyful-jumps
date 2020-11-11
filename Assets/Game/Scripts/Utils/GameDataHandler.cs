using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;

public class GameDataHandler
{
    //public static void storeDataObject(int totalScore, int levelsUnlocked)
    //{

    //    Dictionary<string, string> data = new Dictionary<string, string>();
    //    data.Add("total-eggs", totalScore.ToString());
    //    data.Add("level", levelsUnlocked.ToString());

    //    if (YipliHelper.checkInternetConnection())
    //    {
    //        Debug.Log("returnTotalScore" + totalScore);
    //        Debug.Log("returnLevelsUnlocked" + levelsUnlocked);
    //        PlayerSession.Instance.UpdateGameData(data);

    //        Debug.Log("Player Data Saved!");
    //    }

    //}

    public static PlayerGameData GetPlayerData()
    {
        PlayerGameData data = new PlayerGameData();
        DataSnapshot dataSnapshot = PlayerSession.Instance.GetGameData();
        try
        {
            if (dataSnapshot.Value != null)
            {
                data.SetTotalScore(int.Parse(dataSnapshot.Child("reward-points").Value?.ToString()));
                data.SetCurrentLevel(int.Parse(dataSnapshot.Child("current-level").Value?.ToString()));
            }
            else
            {
                data.SetCurrentLevel(0);
                data.SetTotalScore(0);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Something is wrong : " + e.Message);
            data.SetCurrentLevel(0);
            data.SetTotalScore(0);
        }

        return data;
    }
}
