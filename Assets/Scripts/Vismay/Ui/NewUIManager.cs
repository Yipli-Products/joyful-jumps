using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewUIManager : MonoBehaviour
{
    // required variables
    [Header("UI Elements")]
    [SerializeField] private Button mainCommonButton = null;
    [SerializeField] private TextMeshProUGUI commonButtonText = null;

    // current yipli config
    [Header("Scriptables requirements")]
    [SerializeField] YipliConfig currentyipliConfig = null;

    [Header("Required Script objects")]
    [SerializeField] private PlayerSelection playerSelection = null;
    [SerializeField] private MatSelection matSelection = null;

    public string currentPanelTag = string.Empty;
    private bool currentIsMainTanenceModeOn = false;

    /* All Panels and Buttons Tags */

    //Panels----------
    const string playerSelectionPanel = "PlayerSelectionPanel";
    const string noInternetPanel = "NoInternetPanel";
    const string noMatPanel = "NoMatPanel"; // guest user panel
    const string launchFromYipliAppPanel = "LaunchFromYipliAppPanel";
    const string maintanencePanel = "MaintanencePanel";
    const string noMatConnectionPanel = "NoMatConnectionPanel";
    const string phoneHolderTutorialPanel = "PhoneHolderTutorialPanel";

    //Buttons---------

    const string mainCommonButtonTag = "mainCommonButton";

    void Start() {
        TurnOffMainCommonButton();
    }

    public void UpdateButtonDisplay(string panelTag, bool isMainTanenceModeOn = false) {
        switch(panelTag) {
            case playerSelectionPanel:
                currentPanelTag = playerSelectionPanel;
                break;

            case noInternetPanel:
                currentPanelTag = noInternetPanel;

                commonButtonText.text = "Try Again";
                TurnOnMainCommonButton();
                TurnMainButtonScaleToOne();
                break;

            case noMatPanel:
                currentPanelTag = noMatPanel;

                commonButtonText.text = "Get Mat"; // check if we can use BUY as button text
                TurnOnMainCommonButton();
                TurnMainButtonScaleToOne();
                break;

            case launchFromYipliAppPanel:
                currentPanelTag = launchFromYipliAppPanel;
                break;

            case maintanencePanel:
                currentPanelTag = maintanencePanel;

                currentIsMainTanenceModeOn = isMainTanenceModeOn;

                if (isMainTanenceModeOn) {
                    commonButtonText.text = "Quit";
                } else {
                    commonButtonText.text = "Update";
                }

                TurnOnMainCommonButton();
                TurnMainButtonScaleToOne();
                break;

            case noMatConnectionPanel:
                currentPanelTag = noMatConnectionPanel;

                commonButtonText.text = "Recheck";
                TurnOnMainCommonButton();
                TurnMainButtonScaleToOne();
                break;

            case phoneHolderTutorialPanel:
                currentPanelTag = phoneHolderTutorialPanel;

                commonButtonText.text = "Jump";
                TurnOnMainCommonButton();
                TurnMainButtonScaleToZero();
                break;

            default:
                break;
        }
    }

    public void MainButtonFucntion() {
        switch(currentPanelTag) {
            case playerSelectionPanel:
                break;

            case noInternetPanel:
                playerSelection.TryAgainInternetConnection();
                break;

            case noMatPanel:
                Application.OpenURL(ProductMessages.GetMatUrl);
                break;

            case launchFromYipliAppPanel:
                break;

            case maintanencePanel:
                if (currentIsMainTanenceModeOn) {
                    // quit application.
                    Application.Quit();
                } else {
                    // update game
                    playerSelection.OnUpdateGameClick();
                }
                break;

            case noMatConnectionPanel:
                matSelection.ReCheckMatConnection();
                break;

            case phoneHolderTutorialPanel:
                TurnMainButtonScaleToOne();
                playerSelection.OnJumpOnMat();
                break;

            default:
                break;
        }
    }

    public void TurnOnMainCommonButton() {
        mainCommonButton.gameObject.SetActive(true);
    }

    public void TurnOffMainCommonButton() {
        mainCommonButton.gameObject.SetActive(false);
    }

    public void TurnMainButtonScaleToZero() {
        mainCommonButton.transform.localScale = new Vector3(0f, 0f, 0f);
    }

    public void TurnMainButtonScaleToOne() {
        mainCommonButton.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
