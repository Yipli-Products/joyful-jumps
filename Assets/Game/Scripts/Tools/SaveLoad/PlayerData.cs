using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public static int CurrentLevel
    {
        get
        {
            return PlayerPrefs.GetInt("CurrentLevel", 0);
        }
        set
        {
            PlayerPrefs.SetInt("CurrentLevel", value);
            PlayerPrefs.Save();
        }
    }

    public static int SelectedAvatarIndex
    {
        get
        {
            return PlayerPrefs.GetInt("SelectedAvatarIndex", 0);
        }
        set
        {
            PlayerPrefs.SetInt("SelectedAvatarIndex", value);
            PlayerPrefs.Save();
        }
    }

    public static int RewardCoin
    {
        get
        {
            return PlayerPrefs.GetInt("RewardCoin", 0);
        }
        set
        {
            PlayerPrefs.SetInt("RewardCoin", value);
            PlayerPrefs.Save();
        }
    }

    public static long LastSavedTimeStamp
    {
        get
        {
            return long.Parse(PlayerPrefs.GetString("LastSavedTimeStamp", "-1"));
        }
        set
        {
            PlayerPrefs.SetString("LastSavedTimeStamp", value.ToString());
            PlayerPrefs.Save();
        }
    }

    public static string PlayerID
    {
        get
        {
            return PlayerPrefs.GetString("PlayerID", "");
        }
        set
        {
            PlayerPrefs.SetString("PlayerID", value);
            PlayerPrefs.Save();
        }
    }


    public static bool Sound
    {
        get
        {
            int accept = PlayerPrefs.GetInt("Sound", 1);
            return (accept == 1) ? true : false;
        }
        set
        {
            PlayerPrefs.SetInt("Sound", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool PrivacyPolicyAccepted
    {
        get
        {
            int accept = PlayerPrefs.GetInt("PrivacyPolicyAccepted", 0);
            return (accept == 1) ? true : false;
        }
        set
        {
            PlayerPrefs.SetInt("PrivacyPolicyAccepted", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool IsAvatarPurchased(int avatarID)
    {
        int purchased = PlayerPrefs.GetInt(string.Format("AVATAR_{0}", avatarID), 0);
        return (purchased == 1) ? true : false;
    }

    public static void SetAvatarPurchased(int avatarID)
    {
        PlayerPrefs.SetInt(string.Format("AVATAR_{0}", avatarID), 1);
        PlayerPrefs.Save();
    }

    public static int FootStep
    {
        get
        {
            return PlayerPrefs.GetInt("FootStep", 0);
        }
        set
        {
            PlayerPrefs.SetInt("FootStep", value);
            PlayerPrefs.Save();
        }
    }

    public static int JumpStep
    {
        get
        {
            return PlayerPrefs.GetInt("JumpStep", 0);
        }
        set
        {
            PlayerPrefs.SetInt("JumpStep", value);
            PlayerPrefs.Save();
        }
    }

    public static int FallCount
    {
        get
        {
            return PlayerPrefs.GetInt("FallCount", 0);
        }
        set
        {
            PlayerPrefs.SetInt("FallCount", value);
            PlayerPrefs.Save();
        }
    }
}
