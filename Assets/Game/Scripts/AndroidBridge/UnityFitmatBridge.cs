using GodSpeedGames.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityFitmatBridge : PersistentSingleton<UnityFitmatBridge>
{
    //Extend this lister to other script where you want to get response
    public static Action<string> OnGotActionFromBridge;

    string FMResponseCount = "";

    private bool cancontrol = true;
    private float _deltaLag;

    private long _previousTime = 0;
    private long _currentTime = 0;

    private int _latestFootSteps = 0;

    private static bool normalInput = false;


    public void EnableGameInput()
    {
        normalInput = true;
        if (PlayerSession.Instance != null)
            PlayerSession.Instance.SetGameClusterId(1);
    }

    public void DisableGameInput()
    {
        normalInput = false;
        if (PlayerSession.Instance != null)
            PlayerSession.Instance.SetGameClusterId(0);
    }

    protected virtual void Start()
    {
        DisableGameInput();

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        _previousTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    /*protected virtual void Update()
    {
        _currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if ((_currentTime - _previousTime) >= 1000 || normalInput)
        {
            string FMResponse = InitBLE.PluginClass.CallStatic<string>("_getFMResponse");

            string[] FMTokens = FMResponse.Split('.');

            if (FMTokens.Length > 0 && !FMTokens[0].Equals(FMResponseCount))
            {
                Debug.Log("FMResponse " + FMResponse);
                FMResponseCount = FMTokens[0];

                if (FMTokens.Length > 1)
                {
                    string[] whiteSpace = FMTokens[1].Split(' ');

                    if (whiteSpace.Length > 1)
                    {
                        int n;
                        bool isNumeric = int.TryParse(whiteSpace[1], out n);

                        if (isNumeric)
                        {
                            _latestFootSteps = n;
                        }
                        else
                        {
                            int step = PlayerData.FootStep;
                            PlayerData.FootStep = step + _latestFootSteps;

                            whiteSpace[0] = FMTokens[1];

                            _latestFootSteps = 0;
                        }
                    }

                    OnGotActionFromBridge?.Invoke(whiteSpace[0]);
                }
            }

            _previousTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }*/

    protected virtual void Update()
    {
        _currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        if ((_currentTime - _previousTime) >= 750 || normalInput)
        {
            string FMResponse = InitBLE.PluginClass.CallStatic<string>("_getFMResponse");

            string[] FMTokens = FMResponse.Split('.');

            if (FMTokens.Length > 0 && !FMTokens[0].Equals(FMResponseCount))
            {
                Debug.Log("FMResponse " + FMResponse);
                FMResponseCount = FMTokens[0];

                if (FMTokens.Length > 1)
                {
                    string[] whiteSpace = FMTokens[1].Split('+');

                    if (whiteSpace.Length > 1)
                    {
                        int step = PlayerData.FootStep;
                        PlayerData.FootStep = step + int.Parse(whiteSpace[1]);
                    }

                    OnGotActionFromBridge?.Invoke(whiteSpace[0]);
                }
            }

            _previousTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (PlayerSession.Instance != null)
            PlayerSession.Instance.UpdateDuration();
    }

    public void GotResponseFromBridge(string message)
    {
        Debug.Log("GotResponseFromBridge " + message);
        OnGotActionFromBridge?.Invoke(message);
    }
}
