using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YipliFMDriverCommunication;
using TMPro;
using System.Collections;
using System;

public class TutorialManagerGL : MonoBehaviour
{
    // required variables
    [Header("Interactive tutorial components")]
    [SerializeField] List<Button> tuorialButtons;
    [SerializeField] Button currentB;

    [SerializeField] TextMeshProUGUI instructionOne;
    [SerializeField] TextMeshProUGUI instructionTwo;
    [SerializeField] TextMeshProUGUI finalInstruction;

    [SerializeField] TextMeshProUGUI leftTaps;
    [SerializeField] TextMeshProUGUI rightTaps;
    [SerializeField] TextMeshProUGUI buttonClicks;

    [SerializeField] MatInputController matInputController;

    [SerializeField] RawImage buttonOneImg;
    [SerializeField] RawImage buttonTwoImg;
    [SerializeField] RawImage buttonThreeImg;

    [Header("NonInteractive tutorial components")]
    [SerializeField] TextMeshProUGUI instructionsText;

    [SerializeField] RawImage standAnimationImg;
    [SerializeField] RawImage leftTapAnimationImg;
    [SerializeField] RawImage rightTapAnimationImg;

    [Header("Tutorial Panels")]
    [SerializeField] GameObject interactiveTutPanel;
    [SerializeField] GameObject nonInteractiveTutPanel;

    [Header("Main Titles")]
    [SerializeField] TextMeshProUGUI panelTitleText;
    [SerializeField] TextMeshProUGUI panelInstructionTitleText;

    [Header("pauseANimation")]
    [SerializeField] GameObject pauseAnimationOBJ;

    [Header("current yipli config")]
    public YipliConfig currentYipliConfig;

    const string LEFT = "left";
    const string RIGHT = "right";
    const string ENTER = "enter";

    int currentButtonIndex = 0;
    int totalLeftTaps = 0;
    int totalRightTaps = 0;

    YipliUtils.PlayerActions detectedAction;

    bool tappingsDone = false;

    bool buttonOneClicked = false;
    bool buttonTwoClicked = false;
    bool buttonThreeClicked = false;

    char[] textToDisplayArray = null;

    private string[] leftTapsText = {
        "Perfect Left Tap",
        "That's a Left Tap",
        "Good Tap",
        "Left Tap done nicely"
    };

    private string[] rightTapsText = {
        "Great Right Tap",
        "That's a magnificient Right Tap",
        "Good Tap",
        "Right Tap done perfectly"
    };

    public YipliUtils.PlayerActions DetectedAction { get => detectedAction; set => detectedAction = value; }
    public string[] LeftTapsText { get => leftTapsText; set => leftTapsText = value; }
    public string[] RightTapsText { get => rightTapsText; set => rightTapsText = value; }

    private void Start()
    {
        TurnOffInteractiveTutorialChildren();

        instructionOne.GetComponent<Animator>().enabled = false;
    }

    private void TurnOffInteractiveTutorialChildren()
    {
        leftTaps.transform.localScale = new Vector3(0f, 0f, 0f);

        for (int i = 0; i < leftTaps.transform.childCount; i++)
        {
            leftTaps.transform.GetChild(i).transform.localScale = new Vector3(0f, 0f, 0f);
        }

        rightTaps.transform.localScale = new Vector3(0f, 0f, 0f);

        for (int i = 0; i < rightTaps.transform.childCount; i++)
        {
            rightTaps.transform.GetChild(i).transform.localScale = new Vector3(0f, 0f, 0f);
        }

        buttonClicks.transform.localScale = new Vector3(0f, 0f, 0f);

        for (int i = 0; i < buttonClicks.transform.childCount; i++)
        {
            buttonClicks.transform.GetChild(i).transform.localScale = new Vector3(0f, 0f, 0f);
        }

        instructionOne.text = "";
        instructionTwo.text = "";

        leftTaps.gameObject.SetActive(false);
        rightTaps.gameObject.SetActive(false);
        buttonClicks.gameObject.SetActive(false);

        pauseAnimationOBJ.transform.GetChild(0).gameObject.SetActive(true);
        pauseAnimationOBJ.SetActive(false);

        foreach (Button b in tuorialButtons)
        {
            b.transform.localScale = new Vector3(0f, 0f, 0f);
            b.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (matInputController.IsTutorialRunning)
        {
            GetMatUIKeyboardInputs();
            ManageMatActions();
        }
    }

    public void ActivateTutorial()
    {
        StopCoroutine(ActivateNITutorial());
        StartCoroutine(ActivateNITutorial());
    }

    private IEnumerator ActivateNITutorial()
    {
        //start part
        interactiveTutPanel.SetActive(false);
        leftTapAnimationImg.gameObject.SetActive(false);
        rightTapAnimationImg.gameObject.SetActive(false);

        textToDisplayArray = "MAT Controls Demo".ToCharArray();
        panelTitleText.text = "";

        foreach (char c in textToDisplayArray)
        {
            panelTitleText.text += c.ToString();
            yield return new WaitForSecondsRealtime(0.1f);
        }

        panelTitleText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "___________________";

        standAnimationImg.gameObject.SetActive(true);
        nonInteractiveTutPanel.SetActive(true);

        instructionsText.text = "Stand comfortably on the MAT";

        // second part
        instructionsText.text = "To navigate - left/right taps";

        standAnimationImg.gameObject.SetActive(false);

        leftTapAnimationImg.gameObject.SetActive(true);
        rightTapAnimationImg.gameObject.SetActive(false);

        // wait for tap animations to finish 1 time. 1 is 9 seconds
        yield return new WaitForSecondsRealtime(9f);

        // final part
        panelInstructionTitleText.text = "";
        nonInteractiveTutPanel.SetActive(false);

        StartInteractiveTutorialCoroutine();
    }

    private void StartInteractiveTutorialCoroutine()
    {
        StopCoroutine(ActivateNITutorial());

        StartCoroutine(AnimateInteractiveTutorialIntro());
    }

    private IEnumerator AnimateInteractiveTutorialIntro()
    {
        textToDisplayArray = "MAT Controls Training".ToCharArray();
        panelTitleText.text = "";

        foreach (char c in textToDisplayArray)
        {
            panelTitleText.text += c.ToString();
            yield return new WaitForSecondsRealtime(0.1f);
        }

        panelTitleText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_____________________";

        interactiveTutPanel.SetActive(true);

        instructionTwo.text = "1. To navigate - ";
        ChangeLeftRighttext();

        leftTaps.gameObject.SetActive(true);
        rightTaps.gameObject.SetActive(true);

        while (leftTaps.transform.localScale.x < 1)
        {
            leftTaps.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            yield return new WaitForSecondsRealtime(0.05f);
        }

        for (int i = 0; i < leftTaps.transform.childCount; i++)
        {
            while (leftTaps.transform.GetChild(i).transform.localScale.x < 1)
            {
                leftTaps.transform.GetChild(i).transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }

        while (rightTaps.transform.localScale.x < 1)
        {
            rightTaps.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            yield return new WaitForSecondsRealtime(0.05f);
        }

        for (int i = 0; i < rightTaps.transform.childCount; i++)
        {
            while (rightTaps.transform.GetChild(i).transform.localScale.x < 1)
            {
                rightTaps.transform.GetChild(i).transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }

        foreach (Button b in tuorialButtons)
        {
            b.gameObject.SetActive(true);

            while (b.transform.localScale.x < 1)
            {
                b.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }


        ActivateITutorial();
    }

    private void ChangeLeftRighttext()
    {
        instructionOne.text = "Left/Right tap";
    }

    public void ActivateITutorial()
    {
        matInputController.IsTutorialRunning = true;

        currentButtonIndex = 1;
        ChangeButtonText();
        ManageCurrentButton();

        finalInstruction.gameObject.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.L))
        {
            ProcessMatInputs(ENTER);
        }
    }

    private void ManageMatActions()
    {
        string fmActionData = InitBLE.GetFMResponse();
        Debug.Log("Json Data from Fmdriver : " + fmActionData);

        FmDriverResponseInfo singlePlayerResponse = JsonUtility.FromJson<FmDriverResponseInfo>(fmActionData);

        if (singlePlayerResponse == null) return;

        if (PlayerSession.Instance.currentYipliConfig.oldFMResponseCount != singlePlayerResponse.count)
        {
            PlayerSession.Instance.currentYipliConfig.oldFMResponseCount = singlePlayerResponse.count;

            DetectedAction = ActionAndGameInfoManager.GetActionEnumFromActionID(singlePlayerResponse.playerdata[0].fmresponse.action_id);

            switch (DetectedAction)
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

    private void ManageCurrentButton()
    {
        for (int i = 0; i < tuorialButtons.Count; i++)
        {
            if (i == currentButtonIndex)
            {
                // animate button
                tuorialButtons[i].GetComponent<Animator>().enabled = true;
                tuorialButtons[i].transform.GetChild(0).gameObject.SetActive(true);
                currentB = tuorialButtons[i];
            }
            else
            {
                // do nothing
                tuorialButtons[i].transform.localScale = new Vector3(1f, 1f, 1f);
                tuorialButtons[i].GetComponent<Animator>().enabled = false;
                tuorialButtons[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void ProcessMatInputs(string matInput)
    {
        switch (matInput)
        {
            case LEFT:
                currentButtonIndex = GetPreviousButton();

                if (!tappingsDone)
                {
                    ChangeButtonText();
                }

                ManageCurrentButton();
                ProgressTutorial(LEFT);
                break;

            case RIGHT:
                currentButtonIndex = GetNextButton();

                if (!tappingsDone)
                {
                    ChangeButtonText();
                }

                ManageCurrentButton();
                ProgressTutorial(RIGHT);
                break;

            case ENTER:
                ProgressTutorial(ENTER);
                break;

            default:
                Debug.Log("Wrong Input");
                break;
        }
    }

    private int GetNextButton()
    {
        if ((currentButtonIndex + 1) == tuorialButtons.Count)
        {
            return 0;
        }
        else
        {
            return currentButtonIndex + 1;
        }
    }

    private int GetPreviousButton()
    {
        if (currentButtonIndex == 0)
        {
            return tuorialButtons.Count - 1;
        }
        else
        {
            return currentButtonIndex - 1;
        }
    }

    private void ProgressTutorial(string providedAction)
    {
        if((providedAction == LEFT || providedAction == RIGHT) && !tappingsDone)
        {
            if (providedAction == LEFT)
            {
                totalLeftTaps++;
                //instructionOne.text = "";
                //instructionTwo.text = "";

                //allowChangeText = false;
                //StopCoroutine(ChangeLeftRighttext());

                instructionOne.GetComponent<Animator>().enabled = false;
                //instructionTwo.text = LeftTapsText[Random.Range(0, LeftTapsText.Length)];

                ProgressCheckMarks(totalLeftTaps, leftTaps.gameObject);
            }
            else
            {
                totalRightTaps++;
                //instructionOne.text = "";
                //instructionTwo.text = "";

               // allowChangeText = false;
               // StopCoroutine(ChangeLeftRighttext());

                instructionOne.GetComponent<Animator>().enabled = false;
                //instructionTwo.text = RightTapsText[Random.Range(0, RightTapsText.Length)];

                ProgressCheckMarks(totalRightTaps, rightTaps.gameObject);
            }

            if (totalLeftTaps >= 3 && totalRightTaps >= 3)
            {
                instructionOne.text = "";
                instructionTwo.text = "";

                StartCoroutine(TeachJump());
            }
        }
        else if (providedAction == ENTER && tappingsDone)
        {
            ClickButton();
        }
    }

    public void ClickButton()
    {
        currentB.onClick.Invoke();
    }

    public void ContinueButton()
    {
        matInputController.IsTutorialRunning = false;
        //currentB.onClick.Invoke();
    }

    private void ProgressCheckMarks(int caseNumber, GameObject effectThisObject)
    {
        switch (caseNumber)
        {
            case 1:
                StartCoroutine(FillImage(effectThisObject.transform.GetChild(0).GetComponent<Image>(), effectThisObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>()));
                break;

            case 2:
                StartCoroutine(FillImage(effectThisObject.transform.GetChild(1).GetComponent<Image>(), effectThisObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>()));
                break;

            case 3:
                StartCoroutine(FillImage(effectThisObject.transform.GetChild(2).GetComponent<Image>(), effectThisObject.transform.GetChild(2).transform.GetChild(0).GetComponent<Image>()));
                break;

            default:
                Debug.LogError(effectThisObject.name + " taps number is not 1,2,3 : number is : " + caseNumber);
                break;
        }
    }

    private IEnumerator TeachJump()
    {
        leftTaps.gameObject.SetActive(false);
        rightTaps.gameObject.SetActive(false);

        foreach (Button b in tuorialButtons)
        {
            b.gameObject.SetActive(false);
        }

        textToDisplayArray = "2. To select - ".ToCharArray();

        instructionTwo.text = "";

        foreach (char c in textToDisplayArray)
        {
            instructionTwo.text += c.ToString();
            yield return new WaitForSecondsRealtime(0.1f);
        }
        instructionOne.text = "Jump";
        instructionOne.GetComponent<Animator>().enabled = false;

        yield return new WaitForSecondsRealtime(1f);

        instructionOne.GetComponent<Animator>().enabled = false;

        foreach (Button b in tuorialButtons)
        {
            b.gameObject.SetActive(true);
            b.GetComponentInChildren<TextMeshProUGUI>().text = "Select\nMe";
        }


        buttonClicks.gameObject.SetActive(true);

        while (buttonClicks.transform.localScale.x < 1)
        {
            buttonClicks.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            yield return new WaitForSecondsRealtime(0.05f);
        }

        for (int i = 0; i < buttonClicks.transform.childCount; i++)
        {
            while (buttonClicks.transform.GetChild(i).transform.localScale.x < 1)
            {
                buttonClicks.transform.GetChild(i).transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }

        ManageCurrentButton();

        tappingsDone = true;
    }

    public void ButtonOneClick()
    {
        instructionOne.text = "and Select all buttons";
        instructionTwo.text = "3. Now, Navigate";

        buttonOneImg.color = Color.green;
        buttonOneImg.transform.GetChild(0).gameObject.SetActive(true);
        buttonOneImg.transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
        
        StartCoroutine(FillImage(buttonClicks.transform.GetChild(0).GetComponent<Image>(), buttonClicks.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>()));
        
        buttonOneClicked = true;
    }

    public void ButtonTwoClick()
    {
        instructionOne.text = "and Select all buttons";
        instructionTwo.text = "3. Now, Navigate";

        buttonTwoImg.color = Color.green;
        buttonTwoImg.transform.GetChild(0).gameObject.SetActive(true);
        buttonTwoImg.transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";

        StartCoroutine(FillImage(buttonClicks.transform.GetChild(1).GetComponent<Image>(), buttonClicks.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>()));

        buttonTwoClicked = true;
    }

    public void ButtonThreeClick()
    {
        instructionOne.text = "and Select all buttons";
        instructionTwo.text = "3. Now, Navigate";

        buttonThreeImg.color = Color.green;
        buttonThreeImg.transform.GetChild(0).gameObject.SetActive(true);
        buttonThreeImg.transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";

        StartCoroutine(FillImage(buttonClicks.transform.GetChild(2).GetComponent<Image>(), buttonClicks.transform.GetChild(2).transform.GetChild(0).GetComponent<Image>()));

        buttonThreeClicked = true;
    }

    public void StartDirectInteractiveTutorial()
    {
        StopCoroutine(ActivateNITutorial());

        // final part
        nonInteractiveTutPanel.SetActive(false);
        interactiveTutPanel.SetActive(true);

        panelInstructionTitleText.text = "";

        StartInteractiveTutorialCoroutine();
    }

    private IEnumerator FillImage(Image squreBox, Image rightArrow)
    {
        rightArrow.fillAmount = 0;

        while (rightArrow.fillAmount < 1f)
        {
            rightArrow.fillAmount += 0.1f;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        squreBox.color = Color.green;

        if (buttonOneClicked && buttonTwoClicked && buttonThreeClicked)
        {
            yield return new WaitForSecondsRealtime(1f);
            StartEndPartOfTheTutorial();
        }
    }

    private void StartEndPartOfTheTutorial()
    {
        StartCoroutine(EndTutorial());
    }

    public void ResetOnBackFlow()
    {
        totalLeftTaps = 0;
        totalRightTaps = 0;

        buttonOneClicked = false;
        buttonTwoClicked = false;
        buttonThreeClicked = false;

        panelInstructionTitleText.text = "";

        TurnOffInteractiveTutorialChildren();

        ActivateTutorial();
    }

    public void ChangeButtonText()
    {
        switch(currentButtonIndex)
        {
            case 0:
                tuorialButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = "I am\nhere";
                //tuorialButtons[0].GetComponentInChildren<TextMeshProUGUI>().lineSpacing = -20;

                tuorialButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = "Right\nTap";
                tuorialButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = "Left\nTap";
                break;

            case 1:
                tuorialButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = "Left\nTap";

                tuorialButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = "I am\nhere";
                //tuorialButtons[1].GetComponentInChildren<TextMeshProUGUI>().lineSpacing = -20;

                tuorialButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = "Right\nTap";
                break;

            case 2:
                tuorialButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = "Right\nTap";
                tuorialButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = "Left\nTap";

                tuorialButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = "I am\nhere";
                //tuorialButtons[2].GetComponentInChildren<TextMeshProUGUI>().lineSpacing = -20;
                break;
        }
    }

    private IEnumerator EndTutorial()
    {
        instructionOne.text = "";
        instructionOne.GetComponent<Animator>().enabled = false;
        instructionTwo.text = "";

        foreach (Button b in tuorialButtons)
        {
            b.gameObject.SetActive(false);
        }

        leftTaps.gameObject.SetActive(false);
        rightTaps.gameObject.SetActive(false);
        buttonClicks.gameObject.SetActive(false);

        panelTitleText.text = "MAT Controls Training";
        panelTitleText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_____________________";
        finalInstruction.gameObject.SetActive(true);

        pauseAnimationOBJ.SetActive(true);
        pauseAnimationOBJ.transform.GetChild(0).gameObject.SetActive(true);
        finalInstruction.text = "4. To Pause game : step out of the MAT";
        finalInstruction.transform.localPosition = new Vector3(0f, 50f, 0f);

        // wait for pause animation to finish 2 times. 1 is 2.5 seconds
        yield return new WaitForSecondsRealtime(5f);

        pauseAnimationOBJ.SetActive(false);
        finalInstruction.transform.localPosition = new Vector3(0f, -50f, 0f);

        panelTitleText.text = "Congratulations";
        panelTitleText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_______________";
        finalInstruction.text = "You have completed \"MAT Controls Tutorial\"\nAll set !!!";
        finalInstruction.fontSize = 36;

        yield return new WaitForSecondsRealtime(5f);

        // tutorial is ended and moving to player selection
        TutorialDone();
    }

    public void TutorialDone()
    {
        matInputController.IsTutorialRunning = false;

        //FirebaseDBHandler.UpdateTutStatusData(currentYipliConfig.userId, currentYipliConfig.playerInfo.playerId, 1);

        FindObjectOfType<PlayerSelection>().OnTutorialContinuePress();
    }
}