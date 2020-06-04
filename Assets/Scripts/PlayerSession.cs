using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSession : MonoBehaviour
{
    //TODO : remove below:
    string retStatus;
    private string userId = ""; // to be recieved from Yipli
    private string gameId = ""; // to be assigned to every game.
    private string playerId = ""; // to be recieved from Yipli for each game
    private float points; // Game points / coins
    private string playerAge = ""; //Current age of the player
    private string playerHeight = ""; //Current height of the player
    private string playerWeight = ""; //Current height of the player
    private string matId = "";
    private string matMacAddress;
    private DateTime startTime;
    private DateTime endTime;
    private float duration;
    private string intensityLevel = "low"; // to be decided by the game.
    private IDictionary<string, int> playerActionCounts; // to be updated by the player movements
    private IDictionary<string, string> playerGameData; // to be used to store the player gameData like Highscore, last played level etc.

   
    public TextMeshProUGUI bleErrorText;

    public GameObject BleErrorPanelPrefab;
    private GameObject instantiatedBleErrorPanel;
    private bool bIsBleConnectionCoroutineRunning = false;

    public class PlayerActions
    {
        public static string LEFTMOVE = "left move";
        public static string RIGHTMOVE = "right move";
        public static string JUMP = "jumping";
        public static string STOP = "stop";
        public static string RUNNINGSTOPPED = "running stopped";
        public static string RUNNING = "running";
    }

    [JsonIgnore]
    public YipliConfig currentYipliConfig;

    [JsonIgnore]
    private bool bIsPaused; // to be set, when game is paused.

    [JsonIgnore]
    private static PlayerSession _instance;

    [JsonIgnore]
    public static PlayerSession Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("Destroying current instance of playersession and reinitializing");
            Destroy(gameObject);
            _instance = this;
        }
        else
        {
            _instance = this;
        }

        if (currentYipliConfig.playerInfo == null)
        {
            // Call Yipli_GameLib_Scene
            _instance.currentYipliConfig.callbackLevel = SceneManager.GetActiveScene().name;
            Debug.Log("Updating the callBackLevel Value to :" + _instance.currentYipliConfig.callbackLevel);
            Debug.Log("Loading Yipli scene for player Selection...");
            SceneManager.LoadScene("yipli_lib_scene");
        }
        else
        {
            Debug.Log("Current player is not null.");
        }
    }

    public void Start()
    {
        //Check for Bluetooth connection every 1/10th of a second.
        if (!SceneManager.GetActiveScene().name.Equals("yipli_lib_scene", StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("Starting the BLE routine check in PlayerSession Start()");
            StartCoroutine(CheckBleRoutine());
        }
        else
        {
            Debug.Log("Not Starting the BLE routine check in yipli_lib_scene");
        }
    }

    public string GetCurrentPlayer()
    {
        return currentYipliConfig.playerInfo.playerName;
    }

    public void ChangePlayer()
    {
        _instance.currentYipliConfig.callbackLevel = SceneManager.GetActiveScene().name;
        Debug.Log("Updating the callBackLevel Value to :" + _instance.currentYipliConfig.callbackLevel);
        Debug.Log("Loading Yipli scene for player Selection...");
        SceneManager.LoadScene("yipli_lib_scene");
    }

    public Dictionary<string, dynamic> GetJsonDic()
    {
        Dictionary<string, dynamic> x;
        x = new Dictionary<string, dynamic>();
        x.Add("game-id", gameId );
        x.Add("user-id", userId);
        x.Add("player-id", playerId);
        x.Add("age", playerAge);
        x.Add("points", points.ToString());
        x.Add("player-height", playerHeight);
        x.Add("start-time", startTime);
        x.Add("end-time", endTime);
        x.Add("duration", Convert.ToInt32(duration).ToString());
        x.Add("intensity-level", intensityLevel.ToString());
        x.Add("player-action-counts", playerActionCounts);
        x.Add("mac-address", matMacAddress);
        if (playerGameData != null)
        {
            if (playerGameData.Count > 0)
            {
                x.Add("game-data", playerGameData);
            }
            else
            {
                Debug.Log("Game-data is empty");
            }
        }
        else
        {
            Debug.Log("Game-data is null");
        }

        x.Add("mat-id", matId);
        return x;
    }

    //Pass here name of the game
    public void UpdateGameData(Dictionary<string, string> update)
    {
        if (update != null)
        {
            playerGameData = new Dictionary<string, string>();
            playerGameData = update;
        }
    }

    // Get player game data
    public async Task<DataSnapshot> GetGameData(string gameId)
    {
        DataSnapshot dataSnapShot = await FirebaseDBHandler.GetGameData(
            currentYipliConfig.userId,
            currentYipliConfig.playerInfo.playerId,
            gameId,
            () => { Debug.Log("Got Game data successfully"); }
        );
        return dataSnapShot ?? null;
    }


    //Pass here name of the game
    //TODO: Shift this check to backend.
    public void SetYipliGameId(string strGameId)
    {
        if (strGameId.Equals("unleash"))
        {
            gameId = strGameId;
            SetGameClusterId(2);
            intensityLevel = "medium";
        }
        else if (strGameId.Equals("trapped"))
        {
            gameId = strGameId;
            SetGameClusterId(1);
            intensityLevel = "medium";
        }
        else if (strGameId.Equals("joyfuljumps"))
        {
            gameId = strGameId;
            SetGameClusterId(1);
            intensityLevel = "medium";
        }
        else if (strGameId.Equals("eggcatcher"))
        {
            gameId = strGameId;
            SetGameClusterId(2);
            intensityLevel = "low";
        }
        else if (strGameId.Equals("yiplirunner"))
        {
            gameId = strGameId;
            SetGameClusterId(2);
            intensityLevel = "medium";
        }
        else if (strGameId.Equals("rollingball"))
        {
            gameId = strGameId;
            SetGameClusterId(2);
            intensityLevel = "medium";
        }
        else
        {
            gameId = "";
            intensityLevel = "";
        }
    }

    //First function to be called only once when the game starts()
    public void StartSPSession(string GameId)
    {
        Debug.Log("Starting current player session.");
        userId = currentYipliConfig.userId ?? "";
        playerId = currentYipliConfig.playerInfo.playerId;
        playerAge = currentYipliConfig.playerInfo.playerAge ?? "";
        playerHeight = currentYipliConfig.playerInfo.playerHeight ?? "";
        playerWeight = currentYipliConfig.playerInfo.playerWeight ?? "";
        playerActionCounts = new Dictionary<string, int>();
        points = 0;
        duration = 0;
        bIsPaused = false;
        startTime = DateTime.Now;
        SetYipliGameId(GameId);
        matId = currentYipliConfig.matInfo.matId;
        matMacAddress = currentYipliConfig.matInfo.macAddress;
    }

    //To be used for error handling
    //Call in case of exception while playing game.
    public void CloseSPSession()
    {
        endTime = DateTime.Now;
        points = 0;
        duration = 0;
        Debug.Log("Aborting current player session.");
        //Destroy current player session object
    }

    public void StoreSPSession(float gamePoints)
    {
        Debug.Log("Storing current player session to backend database.");
        points = gamePoints;

        endTime = DateTime.Now;

        if(0 == ValidateSessionBeforePosting()) {
            //Store the session data to backend.
            FirebaseDBHandler.PostPlayerSession(Instance, () => { Debug.Log("Session stored in db"); });
            Debug.Log("Single player session stored successfully.");
        }
        else
        {
            Debug.Log("Session not posted : Validation failed for sessoin data.");
        }
    }

    //Function to validate all the session parameters before writing to DB
    private int ValidateSessionBeforePosting()
    {
        if (gameId == null || gameId == "")
        {
            Debug.Log("gameId is not set");
            return -1;
        }
        if (playerId == null || playerId == "")
        {
            Debug.Log("playerId is not set");
            return -1;
        }
        if (playerActionCounts.Count == 0)
        {
            Debug.Log("playerActionCounts is not set");
            return -1;
        }
        if (duration == 0)
        {
            Debug.Log("duration is 0");
            return -1;
        }
        if (intensityLevel == "")
        {
            Debug.Log("intensityLevel is not set");
            return -1;
        }
        return 0;
    }

    //To be called from GamePause function
    public void PauseSPSession()
    {
        Debug.Log("Pausing current player session.");
        bIsPaused = true; // only set the paused flat to true. Fixed update will take care of halting the time counter
    }

    //To be called from GameResume function
    public void ResumeSPSession()
    {
        Debug.Log("Resuming current player session.");
        bIsPaused = false;
    }

    //to be called from all the player movment actions handled script
    public void AddPlayerAction(string action, int steps = 1)
    {
        Debug.Log("Adding action in current player session.");
        if (playerActionCounts.ContainsKey(action))
            playerActionCounts[action] = playerActionCounts[action] + steps;
        else
            playerActionCounts.Add(action, steps);
    }

    //To be called from GameObject FixedUpdate
    public void UpdateDuration()
    {
        Debug.Log("Updating duration for current player session.");
        if (bIsPaused == false)
        {
            duration += Time.deltaTime;
        }
    }

    public void GoToYipli()
    {
        try
        {
            Debug.Log("Go to Yipli Called");
            string bundleId = "org.hightimeshq.yipli"; //to be changed later
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

            AndroidJavaObject launchIntent = null;

            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
            ca.Call("startActivity", launchIntent);
        }
        catch (AndroidJavaException e)
        {
            //Todo: Redirect to playstore for Yipli App.
            Debug.Log(e);
        }
    }

    public void SetGameClusterId(int gameClusterId)
    {
        InitBLE.setGameClusterID(gameClusterId);
    }

    private IEnumerator CheckBleRoutine()
    {
        int autoRetryBleConnectionCount = 10;
        while (true) {
            yield return new WaitForSeconds(.1f);
            string retStatus = GetBleConnectionStatus();
            Debug.Log("Bluetooth Status : " + retStatus);
            if (!retStatus.Equals("Connected", StringComparison.OrdinalIgnoreCase))
            {
                if(autoRetryBleConnectionCount > 0)
                {
                    Debug.Log("Mat is disconnected. Trying to connect back : " + currentYipliConfig.matInfo.macAddress);
                    InitBLE.InitBLEFramework(currentYipliConfig.matInfo.macAddress);
                    autoRetryBleConnectionCount--;
                }
                else
                {
                    //Exhausted auto reties
                    Debug.Log("Setting the Error Panel Active");
                    bleErrorText.text = "Bluetooth Connection lost. Make sure that the Yipli Mat(default) and your device bluetooth are turned on and ReCheck.";
                    BleErrorPanelPrefab.SetActive(true);
                }
            }
            else
            {
                Debug.Log("Bluetooth connection is established.");
                Debug.Log("Destroying Ble Error canvas prefab.");
                BleErrorPanelPrefab.SetActive(false);
                autoRetryBleConnectionCount = 10;
            }
        }
    }

    public void StartCoroutineForBleReConnection()
    {
        Debug.Log("In StartCoroutineForBleReConnection.");
        if (!bIsBleConnectionCoroutineRunning)
        {
            StartCoroutine(ReconnectBleFromGame());
        }
        else
        {
            Debug.Log("Coroutine : ReconnectBleFromGame is already running");
        }
    }

    private IEnumerator ReconnectBleFromGame()
    {
        Debug.Log("In ReconnectBleFromGame.");
        bIsBleConnectionCoroutineRunning = true;
        bleErrorText.text = "Retrying bluetooth connection...";

        InitBLE.InitBLEFramework(currentYipliConfig.matInfo.macAddress);
        bool bIsConnected = false;
        int iTryCount = 10;
        while (iTryCount > 0) {
            Debug.Log("In ReconnectBleFromGame while : " + iTryCount);
            iTryCount--;
            yield return new WaitForSeconds(.35f);
            string strStatus = GetBleConnectionStatus();
            if (strStatus.Equals("connected", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Connected back");
                bleErrorText.text = "Connected back successfully";
                yield return new WaitForSeconds(.35f);

                BleErrorPanelPrefab.SetActive(false);
                bIsConnected = true;
                iTryCount = 0;
                break;
            }
        }

         if (!bIsConnected)
        {
            //Function execution will come here only if the mat is not connected after the given no. of retries.
            bleErrorText.text = "Bluetooth Connection lost. Make sure that the Yipli Mat(default) and your device bluetooth are turned on and ReCheck.";
        }

        bIsBleConnectionCoroutineRunning = false;
    }

    public string GetBleConnectionStatus()
    {
        return InitBLE.getBLEStatus();
    }
}
