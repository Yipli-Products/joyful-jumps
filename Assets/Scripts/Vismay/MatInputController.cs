using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YipliFMDriverCommunication;

public class MatInputController : MonoBehaviour
{
    // required variables
    [Header("current yipli config")]
    public YipliConfig currentYipliConfig;

    // fix switch cases
    const string LEFT = "left";
    const string RIGHT = "right";
    const string ENTER = "enter";

    // current button this value will keep changing with mat left and right
    [SerializeField] Button currentB;
    [SerializeField] Button mainButton;
    [SerializeField] private List<Button> currentMenuButtons; // list of current buttons for mat controls. This list will change based on current active panel
    [SerializeField] int currentButtonIndex = 0;

    YipliUtils.PlayerActions detectedAction;

    // canvas object for keeping the active button top to the list. Yhis part is only used for PlayerSelection panel.
    [SerializeField] ScrollRect playerSelectionScrollRect;
    [SerializeField] RectTransform playerSelectionContentRectTransform;

    // flag to check if current panel is playerselection panel or not.
    public bool isThisPlayerSelectionPanel = false;
    public bool isThisSwitchPlayerPanel = false;

    // flag to disable the MatControls when tutorial is active.
    bool isTutorialRunning = false;

    // string to store current playername
    string currentPlayerName = null;

    // colors
    [Header("Required Colors")]
    [SerializeField] private Color lightBlueBorder;
    [SerializeField] private Color tangerineBorder;

    [Header("UIMangers")]
    [SerializeField] private NewMatInputController newMatInputController = null;

    [Header("Player Selection Area")]
    [SerializeField] private GameObject playerLeft = null;
    [SerializeField] private GameObject playerMiddle = null;
    [SerializeField] private GameObject playerRight = null;
    [SerializeField] private GameObject switchPlayerPanelPlayerObject = null;
    [SerializeField] private Sprite defaultProfilePic = null;

    // required getters and setters.
    public YipliUtils.PlayerActions DetectedAction { get => detectedAction; set => detectedAction = value; }
    public bool IsThisPlayerSelectionPanel { get => isThisPlayerSelectionPanel; set => isThisPlayerSelectionPanel = value; }
    public bool IsTutorialRunning { get => isTutorialRunning; set => isTutorialRunning = value; }
    public string CurrentPlayerName { get => currentPlayerName; set => currentPlayerName = value; }
    public bool IsThisSwitchPlayerPanel { get => isThisSwitchPlayerPanel; set => isThisSwitchPlayerPanel = value; }

    void Start()
    {
        // set cluster id to 0 as it is the only cluster id needed till main menu arrives.
        SetProperClusterID(0);
        IsThisSwitchPlayerPanel = false;
        IsThisPlayerSelectionPanel = false;
    }

    void Update()
    {
        // mat and keyboard controls will be stopped when tutorial is active.
        if (!IsTutorialRunning)
        {
            GetMatUIKeyboardInputs();
            ManageMatActions();
        }
    }

    public void SetProperClusterID(int clusterID)
    {
        try
        {
            //Debug.LogError("provided clusterID : " + clusterID);
            YipliHelper.SetGameClusterId(clusterID);
        }
        catch (Exception e)
        {
            Debug.LogError("Something went wrong with setting the cluster id : " + e.Message);
        }
    }

    private void ManageMatActions()
    {
        //if (!currentYipliConfig.onlyMatPlayMode) return;
        
        string fmActionData = InitBLE.GetFMResponse();
        Debug.Log("Json Data from Fmdriver in matinput : " + fmActionData);

        FmDriverResponseInfo singlePlayerResponse = JsonUtility.FromJson<FmDriverResponseInfo>(fmActionData);

        if (singlePlayerResponse == null) return;

        if (currentYipliConfig.oldFMResponseCount != singlePlayerResponse.count)
        {
            PlayerSession.Instance.currentYipliConfig.oldFMResponseCount = singlePlayerResponse.count;

            DetectedAction = ActionAndGameInfoManager.GetActionEnumFromActionID(singlePlayerResponse.playerdata[0].fmresponse.action_id);

            switch(DetectedAction)
            {
                // UI input executions
                case YipliUtils.PlayerActions.LEFT:
                    ProcessMatInputs(LEFT);
                    break;

                case YipliUtils.PlayerActions.RIGHT:
                    ProcessMatInputs(RIGHT);
                    break;

                case YipliUtils.PlayerActions.ENTER:
                    ProcessMatInputs(ENTER);
                    break;

                default:
                    Debug.LogError("Wrong Action is detected : " + DetectedAction.ToString());
                    break;
            }
        }
    }

    public void UpdateButtonList(List<Button> newButtons, int newCurrentButtonIndex, bool isPlayerSelectionPanel)
    {
        currentButtonIndex = newCurrentButtonIndex;
        currentMenuButtons = newButtons;

        IsThisPlayerSelectionPanel = isPlayerSelectionPanel;

        ManageCurrentButton(IsThisPlayerSelectionPanel);
    }

    private void ProcessMatInputs(string matInput)
    {
        switch (matInput)
        {
            case LEFT:

                if (IsThisSwitchPlayerPanel) {
                    newMatInputController.SetMatToNormalScale();
                    newMatInputController.HideTextButtons();

                    IsThisSwitchPlayerPanel = false;

                    FindObjectOfType<PlayerSelection>().OnSwitchPlayerPress();
                    return;
                }

                currentButtonIndex = GetPreviousButton();
                ManageCurrentButton(IsThisPlayerSelectionPanel);
                newMatInputController.EnableMatLeftButtonAnimator();
                break;

            case RIGHT:

                if (IsThisSwitchPlayerPanel) {
                    newMatInputController.SetMatToNormalScale();
                    newMatInputController.HideTextButtons();

                    IsThisSwitchPlayerPanel = false;

                    FindObjectOfType<PlayerSelection>().OnContinuePress();
                    return;
                }

                currentButtonIndex = GetNextButton();
                ManageCurrentButton(IsThisPlayerSelectionPanel);
                newMatInputController.EnableMatRightButtonAnimator();
                break;

            case ENTER:

                if (IsThisSwitchPlayerPanel) return;

                if (IsThisPlayerSelectionPanel) {
                    //currentB.onClick.Invoke();
                    currentPlayerName = playerMiddle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    playerMiddle.GetComponent<Button>().onClick.Invoke();
                } else {
                    mainButton.onClick.Invoke();
                }
                break;

            default:
                Debug.Log("Wrong Input");
                break;
        }
    }

    private int GetNextButton()
    {
        if (IsThisPlayerSelectionPanel) {
            if ((currentButtonIndex + 1) == currentYipliConfig.allPlayersInfo.Count)
            {
                return 0;
            }
            else
            {
                return currentButtonIndex + 1;
            }
        } else {
            if ((currentButtonIndex + 1) == currentMenuButtons.Count)
            {
                return 0;
            }
            else
            {
                return currentButtonIndex + 1;
            }
        }
    }

    private int GetPreviousButton()
    {
        if (IsThisPlayerSelectionPanel) {
            if (currentButtonIndex == 0)
            {
                return currentYipliConfig.allPlayersInfo.Count - 1;
            }
            else
            {
                return currentButtonIndex - 1;
            }
        } else {
            if (currentButtonIndex == 0)
            {
                return currentMenuButtons.Count - 1;
            }
            else
            {
                return currentButtonIndex - 1;
            }
        }
    }

    private void ManageCurrentButton(bool isPlayerSelectionPanel)
    {
        Debug.LogError("switchPlayer isPlayerSelectionPanel :  " + isPlayerSelectionPanel);

        if (isPlayerSelectionPanel)
        {
            ScrollButtonList(currentButtonIndex); 
            //FindObjectOfType<swipe>().WhichBtnClicked(currentMenuButtons[currentButtonIndex]);
        }
        /*
        else
        {
            for (int i = 0; i < currentMenuButtons.Count; i++)
            {
                if (i == currentButtonIndex)
                {
                    // animate button
                    currentMenuButtons[i].GetComponent<Animator>().enabled = true;
                    currentB = currentMenuButtons[i];
                }
                else
                {
                    // do nothing
                    currentMenuButtons[i].transform.localScale = new Vector3(1f, 1f, 1f);
                    currentMenuButtons[i].GetComponent<Animator>().enabled = false;
                }
            }
        }
        */
    }

    private void GetMatUIKeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ProcessMatInputs(LEFT);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ProcessMatInputs(RIGHT);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ProcessMatInputs(ENTER);
        }
    }

    /*
    private void ScrollButtonList(Button currentButton, int position)
    {
        Canvas.ForceUpdateCanvases();
        playerSelectionContentRectTransform.anchoredPosition =
            (Vector2)playerSelectionScrollRect.transform.InverseTransformPoint(playerSelectionContentRectTransform.position)
            - (Vector2)playerSelectionScrollRect.transform.InverseTransformPoint(currentButton.transform.position);
        playerSelectionContentRectTransform.anchoredPosition = new Vector2(0f, playerSelectionContentRectTransform.anchoredPosition.y - 50f);
    }
    */

    public void ScrollButtonList(int btnIndex)
    {
        /*
        for (int i = 0; i < currentYipliConfig.allPlayersInfo.Count; i++)
        {
            if (i == btnIndex) {
                UpdateButtonObject(currentYipliConfig.allPlayersInfo[i-1], playerLeft);
                UpdateButtonObject(currentYipliConfig.allPlayersInfo[i], playerMiddle);
                UpdateButtonObject(currentYipliConfig.allPlayersInfo[i+1], playerRight);
            }
        }
        */

        Debug.LogError("switchPlayer button scroll :  " + currentYipliConfig.allPlayersInfo.Count);

        if (currentYipliConfig.allPlayersInfo.Count == 1) {
            UpdateButtonObject(currentYipliConfig.allPlayersInfo[0], playerMiddle);

            playerLeft.SetActive(false);
            playerRight.SetActive(false);
        } else {
            if (btnIndex == 0) {
                UpdateButtonObject(currentYipliConfig.allPlayersInfo[GetPreviousButton()], playerLeft);
            } else {
                UpdateButtonObject(currentYipliConfig.allPlayersInfo[btnIndex - 1], playerLeft);
            }

            UpdateButtonObject(currentYipliConfig.allPlayersInfo[btnIndex], playerMiddle);

            if (btnIndex == currentYipliConfig.allPlayersInfo.Count) {
                UpdateButtonObject(currentYipliConfig.allPlayersInfo[0], playerRight);
            } else {
                UpdateButtonObject(currentYipliConfig.allPlayersInfo[GetNextButton()], playerRight);
            }
        }
    }

    private void UpdateButtonObject(YipliPlayerInfo playerInfo, GameObject playerObject) {
        playerObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerInfo.playerName;
        
        if (playerInfo.playerProfilePicIMG != null) {
            playerObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = playerInfo.playerProfilePicIMG;   
        } else {
            playerObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = defaultProfilePic;
        }

        playerObject.transform.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(FindObjectOfType<PlayerSelection>().SelectPlayer);
    }

    public void UpdateSwitchPlayerPanelPlayerObject() {
        switchPlayerPanelPlayerObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerMiddle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        switchPlayerPanelPlayerObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = playerMiddle.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite;
    }

    public GameObject GetCurrentButton()
    {
        return currentB.gameObject;
    }
}
