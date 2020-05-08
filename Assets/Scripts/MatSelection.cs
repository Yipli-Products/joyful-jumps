using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatSelection : MonoBehaviour
{
    public TextMeshProUGUI NoMatText;
    public TextMeshProUGUI PasswordErrorText;
    public InputField inputPassword;
    public GameObject LoadingPanel;
    private List<YipliMatInfo> Mats = new List<YipliMatInfo>();

    public GameObject NoMatPanel;
    public GameObject SecretEntryPanel;
    public YipliConfig currentYipliConfig;
    private string connectionState;
    private int checkMatStatusCount;

    public void CheckMatConnectionStatus(string user)
    {
        Debug.Log("Checking Mat.");

        NoMatPanel.SetActive(false);

        string connectionState = "";
        if (Mats.Count > 0)
        {
            connectionState = InitBLE.getBLEStatus();
            if (connectionState == "CONNECTED")
            {
                //load last Scene
                SceneManager.LoadScene(currentYipliConfig.callbackLevel);
            }
            else
            {
                Debug.Log("Mat not reachable.");
                NoMatText.text = "Make sure that the registered mat is reachable.";
                NoMatPanel.SetActive(true);
            }
        }
        else //Current Mat not found in Db.
        {
            Debug.Log("No Mat found in cache.");
            NoMatPanel.SetActive(true);
        }
    }

    public void SkipMat()
    {
        NoMatPanel.SetActive(false);
        PasswordErrorText.text = "";
        inputPassword.text = "";
        SecretEntryPanel.SetActive(true);
    }

    public void OnPlayPress()
    {
        if (inputPassword.text == "123456")
        {
            //load last Scene
            SceneManager.LoadScene(currentYipliConfig.callbackLevel);
        }
        else {
            inputPassword.text = "";
            PasswordErrorText.text = "Invalid pasword";
            Debug.Log("incorrect password");
        }
    }

    public void OnBackPress()
    {
        SecretEntryPanel.SetActive(false);
        NoMatPanel.SetActive(true);
    }

    public async void ReCheckMatConnection()
    {
        Debug.Log("Checking Mat.");

        NoMatPanel.SetActive(false);

        //To handle the case of No mats registered
        if (Mats.Count == 0)
        {
            LoadingPanel.SetActive(true);
            await ConnectMat(currentYipliConfig.userId);
            LoadingPanel.SetActive(false);
        }

        string connectionState = "";
        if (Mats.Count > 0)
        {
            connectionState = InitBLE.getBLEStatus();
            if (connectionState == "CONNECTED")
            {
                //load last Scene
                SceneManager.LoadScene(currentYipliConfig.callbackLevel);
            }
            else
            {
                // If it is > 1, reCheckis clicked atleast once. After ReChecking the status, if the status isnt connected,
                //then initiate the Mat connection again, so that, in next reCheck it will get connected.
                InitBLE.InitBLEFramework(Mats[0].matMacId);

                Debug.Log("Mat not reachable.");
                NoMatText.text = "Make sure that the registered mat is reachable.";
                NoMatPanel.SetActive(true);
            }
        }
        else //Current Mat not found in Db.
        {
            Debug.Log("No Mat found in cache.");
            NoMatPanel.SetActive(true);
        }
    }

    public async Task<string> ConnectMat(string userId)
    {
        Debug.Log("Starting mat connection");
        Mats = await FirebaseDBHandler.GetAllMatDetails(userId, () => { Debug.Log("Got the Mat details from db"); });
        if (Mats.Count > 0)
        {
            Debug.Log("No. of mats found : " + Mats.Count);
            Debug.Log("connecting to : " + Mats[0].matName);

            //Initiate the connection with the mat.
            currentYipliConfig.matInfo = Mats[0];
            InitBLE.InitBLEFramework(Mats[0].matMacId);
        }
        else
        {
            Debug.Log("No mat found");
        }
        return "";
    }


    public void OnGoToYipliPress()
    {
        string bundleId = "org.hightimeshq.yipli"; //todo: Change this later
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.Mat.UnityMat");
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
            NoMatText.text = "Yipli App is not installed. Please install Yipli from playstore to proceed.";
        }
    }
}
