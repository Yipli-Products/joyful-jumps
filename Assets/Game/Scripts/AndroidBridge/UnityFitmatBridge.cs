﻿using GodSpeedGames.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YipliFMDriverCommunication;

public class UnityFitmatBridge : PersistentSingleton<UnityFitmatBridge>
{
    //Extend this lister to other script where you want to get response
    public static Action<string> OnGotActionFromBridge;

    int FMResponseCount;

    private bool cancontrol = true;
    private float _deltaLag;

    private long _previousTime = 0;
    private long _currentTime = 0;

    private int _latestFootSteps = 0;

    private static bool normalInput = false;

    public bool bIsInputRevieved = false;

    private bool isGamePaused = false;

    private int currentStepCount = 0;

    // this is to track the step the step count between running && (runningStopped || pause).
    public int CurrentStepCount { get => currentStepCount; set => currentStepCount = value; }

    public bool IsGamePaused { get => isGamePaused; set => isGamePaused = value; }

    public void EnableGameInput()
    {
        Debug.Log("Enabling Game Input");
        normalInput = true;
        if (PlayerSession.Instance != null)
            YipliHelper.SetGameClusterId(1);
    }

    public void DisableGameInput()
    {
        Debug.Log("Disabling Game Input");
        normalInput = false;
        if (PlayerSession.Instance != null)
            YipliHelper.SetGameClusterId(0);
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
        try
        {
            if (SceneManager.GetActiveScene().name == "yipli_lib_scene") return;

            string fmActionData = InitBLE.GetFMResponse();

            Debug.LogError("ClusterID : " + YipliHelper.GetGameClusterId());
            Debug.LogError("Json Data from Fmdriver : " + fmActionData);

            ///if (fmActionData == "No input yet!") return;

            /* New FmDriver Response Format
               {
                  "count": 1,                 # Updates every time new action is detected
                  "timestamp": 1597237057689, # Time at which response was packaged/created by Driver
                  "playerdata": [                      # Array containing player data
                    {
                      "id": 1,                         # Player ID (For Single-player-1 , Multiplayer it could be 1 or 2 )
                      "count" :12,
            "fmresponse": {
                        "action_id": "9D6O",           # Action ID-Unique ID for each action. Refer below table for all action IDs
                        "action_name": "Jump",         # Action Name for debugging (Gamers should strictly check action ID)
                        "properties": "null"           # Any properties action has - ex. Running could have Step Count, Speed
                      }
                    },
                   {
                      "id": 2,                         # Player ID (For Single-player-1 , Multiplayer it could be 1 or 2 )
                      "fmresponse": {
                        "action_id": "9D6O",           # Action ID-Unique ID for each action. Refer below table for all action IDs
                        "action_name": "Jump",         # Action Name for debugging (Gamers should strictly check action ID)
                        "properties": "null"           # Any properties action has - ex. Running could have Step Count, Speed
                      }
                    }
                  ]
                }
            */

            // Json parse FMResponse to get the input.
            FmDriverResponseInfo singlePlayerResponse = JsonUtility.FromJson<FmDriverResponseInfo>(fmActionData);

            if (!IsGamePaused && singlePlayerResponse.playerdata[0].fmresponse.action_id.Equals(ActionAndGameInfoManager.getActionIDFromActionName(YipliUtils.PlayerActions.PAUSE)))
            {
                bIsInputRevieved = true;

                // add running action here
                if (PlayerSession.Instance != null)
                {
                    PlayerSession.Instance.AddPlayerAction(YipliUtils.PlayerActions.RUNNING, CurrentStepCount);
                    Debug.LogError("CurrentStepCount from pause : " + CurrentStepCount);
                    CurrentStepCount = 0;
                }

                OnGotActionFromBridge?.Invoke(ActionAndGameInfoManager.getActionIDFromActionName(YipliUtils.PlayerActions.PAUSE));
            }

            if (PlayerSession.Instance.currentYipliConfig.oldFMResponseCount != singlePlayerResponse.count)
            {
                bIsInputRevieved = true;
                Debug.Log("FMResponse " + fmActionData);
                PlayerSession.Instance.currentYipliConfig.oldFMResponseCount = singlePlayerResponse.count;

                if (singlePlayerResponse.playerdata[0].fmresponse.action_id.Equals(ActionAndGameInfoManager.getActionIDFromActionName(YipliUtils.PlayerActions.RUNNING)))
                {
                    UnityFitmatBridge.Instance.bIsInputRevieved = true;
                    ///CheckPoint for the running action properties.
                    if (singlePlayerResponse.playerdata[0].fmresponse.properties.ToString() != "null")
                    {
                        string[] tokens = singlePlayerResponse.playerdata[0].fmresponse.properties.Split(',');

                        if (tokens.Length > 0)
                        {
                            //Split the property value pairs:
                            string[] totalStepsCountKeyValue = tokens[0].Split(':');
                            if (totalStepsCountKeyValue[0].Equals("totalStepsCount"))
                            {
                                PlayerData.FootStep += int.Parse(totalStepsCountKeyValue[1]);
                                Debug.LogError("Total footSteps : " + PlayerData.FootStep);

                                Debug.LogError("Adding steps : " + totalStepsCountKeyValue[1]);
                                CurrentStepCount = int.Parse(totalStepsCountKeyValue[1]);
                            }

                            string[] speedKeyValue = tokens[1].Split(':');
                            if (speedKeyValue[0].Equals("speed"))
                            {
                                //TODO : Do some handling if speed parameter needs to be used to adjust the running speed in the game.
                            }
                        }
                    }
                }

                OnGotActionFromBridge?.Invoke(singlePlayerResponse.playerdata[0].fmresponse.action_id);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                bIsInputRevieved = false;
                StartCoroutine(ExecutePlayerStop());
            }
        }
        catch(Exception exp)
        {
            Debug.Log("Exception in _getFMResponse processing : " + exp);
        }

        /*string[] FMTokens = FMResponse.Split('.');

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
        */

        // passed frame test
        //Debug.LogError("total frames : " + (Time.frameCount % 5 == 0));
    }

    private IEnumerator ExecutePlayerStop()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.LogError("Runtime platform is A : " + Application.platform);
            yield return new WaitForSecondsRealtime(0.2f);
        }
        else
        {
            Debug.LogError("Runtime platform is W : " + Application.platform);
            yield return new WaitForSecondsRealtime(1f);
        }

        if (false == bIsInputRevieved && YipliHelper.GetGameClusterId() != 0)
        {
            OnGotActionFromBridge?.Invoke(ActionAndGameInfoManager.getActionIDFromActionName(YipliUtils.PlayerActions.RUNNINGSTOPPED));
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
