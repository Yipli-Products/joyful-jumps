using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    public GameObject PlayerSelectionPanel;
    public TextMeshProUGUI ConfirmPlayerText;
    public TextMeshProUGUI ChangePlayerText;
    public TextMeshProUGUI ContinuePlayerText;
    public GameObject PlayerConfirmationPanel;
    public GameObject PlayersContainer;
    public GameObject PlayerChangePanel;
    public GameObject NoPlayerPanel;
    public TextMeshProUGUI NoPlayerText;
    public YipliConfig currentYipliConfig;
    public GameObject PlayerButtonPrefab;
    public GameObject OnlyOnePlayerPanel;
    public GameObject LoadingPanel;

    private List<YipliPLayerInfo> players = new List<YipliPLayerInfo>();
    private string PlayerName;
    private YipliPLayerInfo defaultPlayer;

    public MatSelection matSelectionScript;

    // When the game starts
    public async void Start()
    {
        try
        {
            Debug.Log("In player Selection Start()");
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            currentYipliConfig.userId = intent.Call<string>("getDataString");

            if (!currentYipliConfig.userId.Equals(""))
            {
                UserDataPersistence.SavePropertyValue("user-id", currentYipliConfig.userId);
            }
            else
            {
                currentYipliConfig.userId = UserDataPersistence.GetPropertyValue("user-id");
            }
        }
        catch (System.Exception exp)
        {
            Debug.Log("Exception occured in GetIntent!!!");
            Debug.Log(exp.ToString());
            currentYipliConfig.userId = UserDataPersistence.GetPropertyValue("user-id"); // handling of game directing opening, without yipli app
        }

        if (currentYipliConfig.userId.Equals("") || currentYipliConfig.userId.Equals(null))
        {
            //Go to yipli Panel
            PlayerChangePanel.SetActive(false);
            PlayerSelectionPanel.SetActive(false);
            PlayerConfirmationPanel.SetActive(false);
            OnlyOnePlayerPanel.SetActive(false);
            NoPlayerText.text = "User not found. PLease launch the game from Yipli app once.";
            NoPlayerPanel.SetActive(true);
        }
        else
        {
            LoadingPanel.SetActive(true);
            setBluetoothEnabled(); // Enable the bluetooth first
            string str = await matSelectionScript.ConnectMat(currentYipliConfig.userId); // initiate the mat Connection in advance as it takes time to connect
            LoadingPanel.SetActive(false);

            CheckCurrentPlayer();
        }
    }

    public async void Retry()
    {
        PlayerChangePanel.SetActive(false);
        PlayerSelectionPanel.SetActive(false);
        PlayerConfirmationPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);
        NoPlayerPanel.SetActive(false);
        try
        {
            Debug.Log("In player Selection Start()");
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            currentYipliConfig.userId = intent.Call<string>("getDataString");

            if (!currentYipliConfig.userId.Equals(""))
                UserDataPersistence.SavePropertyValue("user-id", currentYipliConfig.userId);
            else
                currentYipliConfig.userId = UserDataPersistence.GetPropertyValue("user-id");
        }
        catch (System.Exception exp)
        {
            Debug.Log("Exception occured in GetIntent!!!");
            Debug.Log(exp.ToString());
            currentYipliConfig.userId = UserDataPersistence.GetPropertyValue("user-id"); // handling of game directing opening, without yipli app
            //userId = "4B5EfR3JjafcMPFT50YVKE78WF92";
            //userId = "HTdI5n3wJqgHUtszCKqNC70E2OF3";
        }

        if (currentYipliConfig.userId.Equals("") || currentYipliConfig.userId.Equals(null))
        {
            //Go to yipli Panel
            PlayerChangePanel.SetActive(false);
            PlayerSelectionPanel.SetActive(false);
            PlayerConfirmationPanel.SetActive(false);
            OnlyOnePlayerPanel.SetActive(false);
            NoPlayerText.text = "User not found. PLease launch the game from Yipli app once.";
            NoPlayerPanel.SetActive(true);
        }
        else
        {
            LoadingPanel.SetActive(true);
            setBluetoothEnabled(); // Enable the bluetooth first
            string str = await matSelectionScript.ConnectMat(currentYipliConfig.userId); // initiate the mat Connection in advance as it takes time to connect
            LoadingPanel.SetActive(false);
            CheckCurrentPlayer();
        }
    }

    public async void CheckCurrentPlayer()//Call this for every StartGame()/Game Session
    {
        Debug.Log("Checking current player.");

        PlayerConfirmationPanel.SetActive(false);
        PlayerSelectionPanel.SetActive(false);
        PlayerChangePanel.SetActive(false);
        PlayerChangePanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);

        LoadingPanel.SetActive(true);
        //Get Current player details from userId
        defaultPlayer = await FirebaseDBHandler.GetCurrentPlayerdetails(currentYipliConfig.userId, () => { Debug.Log("Got the current player details from db."); });

        LoadingPanel.SetActive(false);
        if (defaultPlayer != null)
        {
            Debug.Log("Found current player : " + defaultPlayer.playerName);
            //This means we have the default Player info from backend.
            //In this case we need to call the player change screen and not the player selection screen
            if (!defaultPlayer.playerName.Equals(""))
            {
                currentYipliConfig.playerInfo = defaultPlayer;
                PlayerChangeFlow();
            }
        }
        else //Current player not found in Db.
        {
            if (currentYipliConfig.playerInfo == null || currentYipliConfig.playerInfo.playerId.Equals("")) // If current Player isn't set in memory
            {
                Debug.Log("No player found in cache. Calling Player selection flow.");
                PlayerChangePanel.SetActive(false);
                PlayerSelectionPanel.SetActive(false);
                PlayerConfirmationPanel.SetActive(false);
                OnlyOnePlayerPanel.SetActive(false);
                NoPlayerText.text = "Create a player from your YIPLI App and make sure there is a default player.";
                NoPlayerPanel.SetActive(true);
            }
            else //Current player is set, call PlayerChangeFlow
            {
                //This means we already have the Current Player info.
                //In this case we need to call the player change screen and not the player selection screen
                PlayerChangeFlow();
            }
        }
    }

    public void PlayerSelectionFlow()
    {
        Debug.Log("In Player selection flow.");
        if (players.Count != 0) //Atleast 1 player found for the corresponding userId
        {
            Debug.Log("Player/s found from firebase : " + players.Count);

            Quaternion spawnrotation = Quaternion.identity;
            Vector3 NextPosition = PlayersContainer.transform.localPosition + new Vector3(0, 50, 0);

            for (int i = 0; i < players.Count; i++)
            {
                GameObject PlayerButton = Instantiate(PlayerButtonPrefab, NextPosition, spawnrotation) as GameObject;
                PlayerButton.name = players[i].playerName;
                PlayerButton.transform.GetChild(0).GetComponent<Text>().text = players[i].playerName;
                PlayerButton.transform.SetParent(PlayersContainer.transform, false);
                PlayerButton.transform.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(SelectPlayer);
                NextPosition = new Vector3(PlayerButton.transform.localPosition.x, PlayerButton.transform.localPosition.y - 70, PlayerButton.transform.localPosition.z);
            }
            PlayerChangePanel.SetActive(false);
            PlayerConfirmationPanel.SetActive(false);
            OnlyOnePlayerPanel.SetActive(false);
            NoPlayerPanel.SetActive(false);
            PlayerSelectionPanel.SetActive(true);
        }
        else
        {
            Debug.Log("No player found from firebase.");
            PlayerChangePanel.SetActive(false);
            PlayerSelectionPanel.SetActive(false);
            PlayerConfirmationPanel.SetActive(false);
            OnlyOnePlayerPanel.SetActive(false);
            NoPlayerText.text = "Create a player from your YIPLI App and make sure there is a default player.";
            NoPlayerPanel.SetActive(true);
        }
    }

    public void PlayerChangeFlow()
    {
        PlayerSelectionPanel.SetActive(false);
        PlayerConfirmationPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);
        NoPlayerPanel.SetActive(false);
        ChangePlayerText.text = "Continue with " + currentYipliConfig.playerInfo.playerName;
        PlayerChangePanel.SetActive(true);
    }

    public void SelectPlayer()
    {
        PlayerName = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log("Player Selected :  " + PlayerName);

        ConfirmPlayerText.text = "Make " + PlayerName + " as current player?";

        PlayerChangePanel.SetActive(false);
        PlayerSelectionPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);
        NoPlayerPanel.SetActive(false);
        PlayerConfirmationPanel.SetActive(true);
    }

    public void OnOkPress()
    {
        Debug.Log("Ok Button Pressed.");
        Debug.Log(PlayerName + " selected");

        PlayerChangePanel.SetActive(false);
        PlayerSelectionPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);
        NoPlayerPanel.SetActive(false);
        PlayerConfirmationPanel.SetActive(false);
        
        currentYipliConfig.playerInfo = GetPlayerInfoFromPlayerName(PlayerName);

        //Make new default player persist to the backend as well, so that it gets reflected in the Yipli App as well.
        FirebaseDBHandler.ChangeCurrentPlayer(currentYipliConfig.userId, currentYipliConfig.playerInfo.playerId, () => { Debug.Log("Change the default player in the backend"); });

        matSelectionScript.CheckMatConnectionStatus(currentYipliConfig.userId);
    }

    private YipliPLayerInfo GetPlayerInfoFromPlayerName(string playerName)
    {
        if (players.Count > 0)
        {
            foreach (YipliPLayerInfo player in players)
            {
                Debug.Log("Found player : " + player.playerName);
                if (player.playerName == playerName)
                {
                    Debug.Log("Found player : " + player.playerName);
                    return player;
                }
            }
        }
        else
        {
            Debug.Log("No Players found.");
        }
        return null;
    }

    public void OnCancelPress()
    {
        Debug.Log("Ok Cancel Pressed.");
        Debug.Log(PlayerName + " selected");
        PlayerConfirmationPanel.SetActive(false);
        PlayerChangePanel.SetActive(false);
        NoPlayerPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);
        PlayerSelectionPanel.SetActive(true);
    }

    public void OnCancelSelectionPress()
    {
        Debug.Log("Cancel selection Pressed.");
        PlayerConfirmationPanel.SetActive(false);
        PlayerSelectionPanel.SetActive(false);
        NoPlayerPanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);
        PlayerChangePanel.SetActive(true);
    }

    public void OnContinuePress()
    {
        Debug.Log("Continue Pressed.");
        Debug.Log(PlayerName + " selected");
        PlayerConfirmationPanel.SetActive(false);
        PlayerSelectionPanel.SetActive(false);
        PlayerChangePanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);
        PlayerChangePanel.SetActive(false);

        matSelectionScript.CheckMatConnectionStatus(currentYipliConfig.userId);
    }

    public async void OnChangePlayerPress()
    {
        PlayerConfirmationPanel.SetActive(false);
        PlayerSelectionPanel.SetActive(false);
        PlayerChangePanel.SetActive(false);
        PlayerChangePanel.SetActive(false);
        OnlyOnePlayerPanel.SetActive(false);

        LoadingPanel.SetActive(true);
        players = await FirebaseDBHandler.GetAllPlayerdetails(currentYipliConfig.userId, currentYipliConfig.playerInfo.playerId, () => { Debug.Log("Got the player details from db"); });

        LoadingPanel.SetActive(false);
        //First check if the players count under userId is more than 1 ?
        if (players.Count > 1)
        {
            PlayerSelectionFlow();
        }
        else // If No then throw a new panel to tell the Gamer that there is only 1 player currently
        {
            PlayerConfirmationPanel.SetActive(false);
            PlayerSelectionPanel.SetActive(false);
            PlayerChangePanel.SetActive(false);
            PlayerChangePanel.SetActive(false);
            ContinuePlayerText.text = "Continue with " + currentYipliConfig.playerInfo.playerName;
            OnlyOnePlayerPanel.SetActive(true);
        }
    }

    public void OnGoToYipliPress()
    {
        string bundleId = "org.hightimeshq.yipli"; //todo: Change this later
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
            ca.Call("startActivity", launchIntent);
        }
        catch (AndroidJavaException e)
        {
            Debug.Log(e);
            NoPlayerText.text = "Yipli App is not installed. Please install Yipli from playstore to proceed.";
        }
    }

    public void setBluetoothEnabled()
    {
        using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            try
            {
                using (var BluetoothManager = activity.Call<AndroidJavaObject>("getSystemService", "bluetooth"))
                {
                    using (var BluetoothAdapter = BluetoothManager.Call<AndroidJavaObject>("getAdapter"))
                    {
                        BluetoothAdapter.Call("enable");
                    }

                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("could not enable the bluetooth automatically");
            }
        }
    }
}
