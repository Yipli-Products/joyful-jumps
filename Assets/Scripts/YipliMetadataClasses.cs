using Firebase.Database;
using System;
using UnityEngine;

public class YipliPLayerInfo
{
    public string playerName;
    public string playerId;
    public string playerAge; //Current age of the player
    public string playerHeight; //Current height of the player
    public string playerWeight; //Current height of the player
    public string playerExpertyLevel;//The Experty level of the player at time of playing the game.
    public string gender;
    public string difficultyLevel; // to be decided by the game.

    public YipliPLayerInfo() { }

    public YipliPLayerInfo(DataSnapshot snapshot, string key)
    {
        if (snapshot != null)
        {
            Debug.Log("filling the YipliPlayerInfo from Snapshot.");
            playerId = key.ToString();
            playerName = snapshot.Child("name").Value?.ToString() ?? "";
            playerWeight = snapshot.Child("weight").Value?.ToString() ?? "";
            playerHeight = snapshot.Child("height").Value?.ToString() ?? "";
            string playerDob = snapshot.Child("dob").Value?.ToString() ?? "";
            Debug.Log("DOB recieved from backend : " + playerDob);
            if (playerDob == "")
            {
                Debug.Log("Player age is null.");
                playerAge = "";
            }
            else
            {
                playerAge = CalculateAge(playerDob);
                Debug.Log("Got Player age : " + playerAge);
            }
            gender = snapshot.Child("gender").Value?.ToString() ?? "";
            Debug.Log("Player Found with details :" + playerAge + " " + playerHeight + " " + playerId + " " + playerWeight + " " + playerName);
        }
    }

    private string CalculateAge(string strDob)
    {
        DateTime now = DateTime.Now;

        DateTime dob = DateTime.Parse(strDob);

        var years = now.Year - dob.Year;

        if (now.Month < dob.Month)
        {
            years--;
        }

        if (now.Month == dob.Month)
        {
            if (now.Day < dob.Day)
            {
                years--;
            }
        }

        return years.ToString();
    }
}

public class YipliMatInfo
{
    public string matName;
    public string matId;
    public string matMacId;

    public YipliMatInfo() { }

    public YipliMatInfo(DataSnapshot snapshot, string key)
    {
        if (snapshot != null)
        {
            Debug.Log("filling the YipliPlayerInfo from Snapshot.");
            matId = key.ToString();
            matName = snapshot.Child("display-name").Value?.ToString() ?? "";
            matMacId = snapshot.Child("mac-address").Value?.ToString() ?? "";
            Debug.Log("Mat Found with details :" + matName + " " + matMacId + " " + matId);
        }
    }
}
