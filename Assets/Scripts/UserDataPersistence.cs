using UnityEngine;

public static class UserDataPersistence
{
    public static void SavePropertyValue(string strProperty, string strValue)
    {
        PlayerPrefs.SetString(strProperty, strValue);
    }

    public static string GetPropertyValue(string strProperty)
    {
        return PlayerPrefs.GetString(strProperty);
    }
}
