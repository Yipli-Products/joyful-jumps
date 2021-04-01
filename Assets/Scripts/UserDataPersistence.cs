using UnityEngine;

public static class UserDataPersistence
{
    public static void SavePropertyValue(string strProperty, string strValue)
    {
        if (strValue.Length > 0)
        {
            PlayerPrefs.SetString(strProperty, strValue);
        }
    }

    public static void DeletePropertyValue(string strProperty)
    {
        PlayerPrefs.DeleteKey(strProperty);
    }

    public static string GetPropertyValue(string strProperty)
    {
        if(PlayerPrefs.HasKey(strProperty) && PlayerPrefs.GetString(strProperty).Length > 0)
            return PlayerPrefs.GetString(strProperty);
        return null;
    }

    public static void SavePlayerToDevice(YipliPlayerInfo playerInfo)
    {
        Debug.Log("Saving player to device with properties : " + playerInfo.playerId + " " + playerInfo.playerName + " " + playerInfo.playerDob + " " + playerInfo.playerHeight + " " + playerInfo.playerWeight);
        SavePropertyValue("player-id", playerInfo.playerId);
        SavePropertyValue("player-name", playerInfo.playerName);
        SavePropertyValue("player-dob", playerInfo.playerDob);
        SavePropertyValue("player-height", playerInfo.playerHeight);
        SavePropertyValue("player-weight", playerInfo.playerWeight);
        SavePropertyValue("player-tutDone", playerInfo.isMatTutDone.ToString());

        if(playerInfo.profilePicUrl != null)
        SavePropertyValue("player-profilePicUrl", playerInfo.profilePicUrl);
        PlayerPrefs.Save();
    }

    private static void ClearPlayerFromDevice()
    {
        DeletePropertyValue("player-id");
        DeletePropertyValue("player-name");
        DeletePropertyValue("player-dob");
        DeletePropertyValue("player-height");
        DeletePropertyValue("player-weight");

        DeletePropertyValue("player-profilePicUrl");
        PlayerPrefs.Save();
    }

    public static void SaveMultiplayerToDevice()
    {
        Debug.Log("Save data test- Starting save data function");
        MultiPlayerData multiPlayerData = PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData;
        try
        {
            if (multiPlayerData.PlayerOneDetails != null)
            {
                Debug.Log("Save data test- Saving player one to device with properties : " + multiPlayerData.PlayerOneDetails.playerId + " " + multiPlayerData.PlayerOneName + " " + multiPlayerData.PlayerOneDetails.playerAge + " " + multiPlayerData.PlayerOneDetails.playerHeight + " " + multiPlayerData.PlayerOneDetails.playerWeight);
                SavePropertyValue("player-one-id", multiPlayerData.PlayerOneDetails.playerId);
                SavePropertyValue("player-one-name", multiPlayerData.PlayerOneName);
                SavePropertyValue("player-one-age", multiPlayerData.PlayerOneDetails.playerAge);
                SavePropertyValue("player-one-height", multiPlayerData.PlayerOneDetails.playerHeight);
                SavePropertyValue("player-one-weight", multiPlayerData.PlayerOneDetails.playerWeight);
            }
            else
            {
                DeletePropertyValue("player-one-id");
                DeletePropertyValue("player-one-name");
                DeletePropertyValue("player-one-age");
                DeletePropertyValue("player-one-height");
                DeletePropertyValue("player-one-weight");
            }
            if (multiPlayerData.PlayerTwoDetails != null)
            {
                Debug.Log("Save data test- Saving player two to device with properties : " + multiPlayerData.PlayerTwoDetails.playerId + " " + multiPlayerData.PlayerTwoName + " " + multiPlayerData.PlayerTwoDetails.playerAge + " " + multiPlayerData.PlayerTwoDetails.playerHeight + " " + multiPlayerData.PlayerTwoDetails.playerWeight);
                SavePropertyValue("player-two-id", multiPlayerData.PlayerTwoDetails.playerId);
                SavePropertyValue("player-two-name", multiPlayerData.PlayerTwoName);
                SavePropertyValue("player-two-age", multiPlayerData.PlayerTwoDetails.playerAge);
                SavePropertyValue("player-two-height", multiPlayerData.PlayerTwoDetails.playerHeight);
                SavePropertyValue("player-two-weight", multiPlayerData.PlayerTwoDetails.playerWeight);
            }
            else
            {
                DeletePropertyValue("player-two-id");
                DeletePropertyValue("player-two-name");
                DeletePropertyValue("player-two-age");
                DeletePropertyValue("player-two-height");
                DeletePropertyValue("player-two-weight");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Saving failed: " + e);
        }
    }

    public static void DeleteMultiplayerDataFromDevice()
    {
        DeletePropertyValue("player-one-id");
        DeletePropertyValue("player-one-name");
        DeletePropertyValue("player-one-age");
        DeletePropertyValue("player-one-height");
        DeletePropertyValue("player-one-weight");
        DeletePropertyValue("player-two-id");
        DeletePropertyValue("player-two-name");
        DeletePropertyValue("player-two-age");
        DeletePropertyValue("player-two-height");
        DeletePropertyValue("player-two-weight");
    }

    public static YipliPlayerInfo GetSavedPlayer()
    {
        Debug.Log("Getting saved player from device.");
        if(GetPropertyValue("player-id") != null && GetPropertyValue("player-name") != null)
            return new YipliPlayerInfo(GetPropertyValue("player-id"),
                GetPropertyValue("player-name"),
                GetPropertyValue("player-dob"), 
                GetPropertyValue("player-height"), 
                GetPropertyValue("player-weight"),
                GetPropertyValue("player-profilePicUrl"),
                YipliHelper.StringToIntConvert(GetPropertyValue("player-tutDone")));

        Debug.Log("Return null for GetSavedPlayer");
        return null;
    }

    public static bool GetSavedMultiplayerFromDevice()
    {
        Debug.Log("Save data test- Getting saved data function");
        if (GetPropertyValue("player-one-id") != null && GetPropertyValue("player-two-id") != null)
        {
            Debug.Log("Save data test- Saved data found");
            MultiPlayerData multiPlayerData = PlayerSession.Instance.currentYipliConfig.MP_GameStateManager.playerData;
            Debug.Log("Save data test- Found scriptable");

            multiPlayerData.PlayerOneDetails.playerId = GetPropertyValue("player-one-id");
            multiPlayerData.PlayerOneName = GetPropertyValue("player-one-name");
            multiPlayerData.PlayerOneDetails.playerId = GetPropertyValue("player-one-age");
            multiPlayerData.PlayerOneDetails.playerId = GetPropertyValue("player-one-height");
            multiPlayerData.PlayerOneDetails.playerId = GetPropertyValue("player-one-weight");
            Debug.Log("Save data test- Got player one data");

            multiPlayerData.PlayerTwoDetails.playerId = GetPropertyValue("player-two-id");
            multiPlayerData.PlayerTwoName = GetPropertyValue("player-two-name");
            multiPlayerData.PlayerTwoDetails.playerId = GetPropertyValue("player-two-age");
            multiPlayerData.PlayerTwoDetails.playerId = GetPropertyValue("player-two-height");
            multiPlayerData.PlayerTwoDetails.playerId = GetPropertyValue("player-two-weight");
            Debug.Log("Save data test- Got player two data");

            return true;
        }
        else
        {
            Debug.Log("Save data test- No saved data found");
            return false;
        }
    }

    public static void SaveMatToDevice(YipliMatInfo matInfo)
    {
        Debug.Log("Saving mat to device with properties : " + matInfo.matId + " " + matInfo.macAddress);
        SavePropertyValue("mat-id", matInfo.matId);
        SavePropertyValue("mac-address", matInfo.macAddress);
        PlayerPrefs.Save();
    }

    public static YipliMatInfo GetSavedMat()
    {
        Debug.Log("Getting saved mat from device.");
        if (GetPropertyValue("mat-id") != null && GetPropertyValue("mac-address") != null)
        {
            return new YipliMatInfo(GetPropertyValue("mat-id"),
                GetPropertyValue("mac-address"));
        }

        Debug.Log("Return null for GetSavedMat");
        return null;
    }

    //If player is not availabe in firebase, delete player's data from device also.
    public static void ClearDefaultPlayer(YipliConfig currentYipliConfig)
    {
        try
        {
            ClearPlayerFromDevice();
            currentYipliConfig.playerInfo = null;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception in Clear player from device : " + e.Message);
        }
    }
}
