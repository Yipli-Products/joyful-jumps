using UnityEngine;
using UnityEngine.UI;
using YipliFMDriverCommunication;
using DG.Tweening;
using System.Collections;
using TMPro;

public class NewMatInputController : MonoBehaviour
{
    // required variables
    [Header("Mat Elelments")]
    [SerializeField] private Image matLeftButton = null;
    [SerializeField] private Image matRightButton = null;
    [SerializeField] private Image matCentreButton = null;

    [SerializeField] private GameObject matParentObj = null;

    [Header("Colors")]
    [SerializeField] private Color originalButtonColor;

    [Header("Vector for positions")]
    [SerializeField] Vector3 playerSelectionPosition;
    [SerializeField] Vector3 tutorialPosition;
    [SerializeField] Vector3 switchPlayerPosition;

    [Header("Chevrons")]
    [SerializeField] GameObject chevronParent = null;

    [Header("Text Buttons")]
    [SerializeField] GameObject textButtonsParent = null;

    [Header("Colors")]
    [SerializeField] private Color yipliRed;
    [SerializeField] private Color yipliBlue;

    public void DisplayMainMat() {
        matParentObj.SetActive(true);
    }

    public void HideMainMat() {
        matParentObj.SetActive(false);
    }

    void Start() {
        matLeftButton.GetComponent<Animator>().enabled = false;
        matRightButton.GetComponent<Animator>().enabled = false;

        //HideTextButtons();
        //HideChevrons();
    }

    public void EnableMatLeftButtonAnimator() {
        matLeftButton.GetComponent<Animator>().enabled = true;
    }

    public void EnableMatRightButtonAnimator() {
        matRightButton.GetComponent<Animator>().enabled = true;
    }

    public void EnableMatParentButtonAnimator() {
        matParentObj.GetComponent<Animator>().enabled = true;
    }

    public void DisableMatParentButtonAnimator() {
        matParentObj.GetComponent<Animator>().enabled = false;;
    }

    public bool GetMatDisplayStatus() {
        return matParentObj.activeSelf;
    }

    public void DisableAnimators() {
        matLeftButton.GetComponent<Animator>().enabled = false;
        matRightButton.GetComponent<Animator>().enabled = false;
        matParentObj.GetComponent<Animator>().enabled = false;

        matLeftButton.color = originalButtonColor;
        matRightButton.color = originalButtonColor;
        //matCentreButton.color = originalButtonColor;
    }

    public void SetMatPlayerSelectionPosition() {
        matParentObj.transform.localPosition = playerSelectionPosition;
    }

    public void SetMatTutorialPosition() {
        matParentObj.transform.localPosition = tutorialPosition;
    }

    public void SetMatSwitchPlayerPosition() {
        matParentObj.transform.localPosition = switchPlayerPosition;
        matParentObj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
    }

    public void SetMatToNormalScale() {
        matParentObj.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void DisplayChevrons() {
        chevronParent.SetActive(true);
    }

    public void HideChevrons() {
        chevronParent.SetActive(false);
    }

    public void DisplayTextButtons() {
        textButtonsParent.SetActive(true);
    }

    public void HideTextButtons() {
        textButtonsParent.SetActive(false);
    }

    public void UpdateCenterButtonColor() {
        matCentreButton.color = yipliRed;
    }

    public void UpdateCenterButtonWithOriginalColor() {
        matCentreButton.color = originalButtonColor;
    }

    public void DisplayMatForSwitchPlayerPanel() {
        textButtonsParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "ReSelect";

        DisplayTextButtons();

        SetMatSwitchPlayerPosition();
    }
}
