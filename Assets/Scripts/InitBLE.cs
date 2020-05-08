using System;
using UnityEngine;

public class InitBLE
{
    static AndroidJavaClass _pluginClass;
    static AndroidJavaObject _pluginInstance;
    const string driverPathName = "com.fitmat.fitmatdriver.Connection.DeviceControlActivity";
    string FMResponseCount = "";
    static string BLEStatus = "";


    //STEP 3 - Create Unity Callback class
    class UnityCallback : AndroidJavaProxy
    {
        private System.Action<string> initializeHandler;
        public UnityCallback(System.Action<string> initializeHandlerIn) : base(driverPathName + "$UnityCallback")
        {
            initializeHandler = initializeHandlerIn;
        }

        public void sendMessage(string message)
        {
            Debug.Log("sendMessage: " + message);
            if (message == "connected")
            {
                InitBLE.BLEStatus = "CONNECTED";
            }
            if (message == "disconnected")
            {
                InitBLE.BLEStatus = "DISCONNECTED";
            }

            if (message == "lost")
            {
                InitBLE.BLEStatus = "CONNECTION LOST";
            }
            if (message.Contains("error"))
            {
                InitBLE.BLEStatus = "ERROR";
            }

            if (initializeHandler != null)
            {
                initializeHandler(message);
            }
        }
    }


    //STEP 4 - Init Android Class & Objects
    public static AndroidJavaClass PluginClass
    {
        get
        {
            if (_pluginClass == null)
            {
                _pluginClass = new AndroidJavaClass(driverPathName);
            }
            return _pluginClass;
        }
    }

    public static AndroidJavaObject PluginInstance
    {
        get
        {
            if (_pluginInstance == null)
            {
                AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance", activity);
            }
            return _pluginInstance;
        }
    }

    public static String getBLEStatus()
    {
        return BLEStatus;
    }

    //STEP 5 - Init Android Class & Objects
    public static void InitBLEFramework(String macaddress)
    {

#if UNITY_IPHONE
                // Now we check that it's actually an iOS device/simulator, not the Unity Player. You only get plugins on the actual device or iOS Simulator.
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    _InitBLEFramework();
                }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            System.Action<string> callback = ((string message) =>
            {
                BLEFramework.Unity.BLEControllerEventHandler.OnBleDidInitialize(message);
            });
            //string macaddress = "A4:34:F1:A5:99:5B";
            //int gameId = 2;
            PluginInstance.Call("_setMACAddress", macaddress);
            PluginInstance.Call("_InitBLEFramework", new object[] { new UnityCallback(callback) });

        }
#endif
    }

    public static void setGameClusterID(int gameID)
    {
        PluginInstance.Call("_setGameID", gameID);
    }

}