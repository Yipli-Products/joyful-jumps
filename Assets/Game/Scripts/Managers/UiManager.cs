using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GodSpeedGames.Tools;

public class UiManager : Singleton<UiManager>
{

    public static System.Action OnBackButtonClicked;
    public static System.Action<MatKey> OnHardwareScreen;
    public static System.Action<MatKey> OnHardwarePopup;

    public enum SCREEN
    {
        None,
        GameScreen,
        MenuScreen,
        GameOverScreen,
        CharacterSelectionScreen,
        TutorialScreen
    }

    public enum Popup
    {
        PauseScreen,
        GameOverScreen,
        LevelFailedScreen,
        SettingsScreen,
        ResetGamePopup
    }

    [SerializeField] Transform screen_content;
    [SerializeField] Transform popup_content;
    [SerializeField] Transform loading_screen_content;
    [SerializeField] Transform transition_screen_content;

    [SerializeField] SCREEN initialScreen;

    public bool forceSendHardwareupdatetoScreen = false;

    private GameObject active_screen;
    private GameObject active_popup;
    private GameObject loading_screen;
    private GameObject transition_screen;

    protected virtual void Start()
    {
        LoadUI(initialScreen);
    }

    private void OnEnable()
    {
        InputController.OnGotKey += GotKeyPressed;
    }

    private void OnDisable()
    {
        InputController.OnGotKey -= GotKeyPressed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (active_popup || loading_screen || transition_screen)
                return;

            OnBackButtonClicked?.Invoke();
        }
    }

    public GameObject LoadUI(SCREEN screen)
    {
        if (screen == SCREEN.None)
            return null;

        CheckForActiveScreen();
        CheckForActivePopup();

        GameObject temp = Resources.Load<GameObject>("UI/Screen/" + screen.ToString());
        active_screen = Instantiate(temp) as GameObject;
        active_screen.transform.SetParent(screen_content);

        RectTransform panel = (RectTransform)active_screen.transform;
        panel.transform.localPosition = Vector3.zero;
        panel.sizeDelta = Vector2.zero;

        active_screen.transform.localScale = Vector3.one;

        if (!active_screen.activeSelf)
            active_screen.SetActive(true);

        System.GC.Collect();

        return active_screen;
    }

    public void LoadLoadingScreen()
    {
        GameObject temp = Resources.Load<GameObject>("UI/Screen/LoadingScreen");
        loading_screen = Instantiate(temp) as GameObject;
        loading_screen.transform.SetParent(loading_screen_content);

        RectTransform panel = (RectTransform)loading_screen.transform;
        panel.transform.localPosition = Vector3.zero;
        panel.sizeDelta = Vector2.zero;

        loading_screen.transform.localScale = Vector3.one;

        if (!loading_screen.activeSelf)
            loading_screen.SetActive(true);
    }

    public void LoadTransitionScreen()
    {
        GameObject temp = Resources.Load<GameObject>("UI/Screen/TransitionScreen");
        transition_screen = Instantiate(temp) as GameObject;
        transition_screen.transform.SetParent(transition_screen_content);

        RectTransform panel = (RectTransform)transition_screen.transform;
        panel.transform.localPosition = Vector3.zero;
        panel.sizeDelta = Vector2.zero;

        transition_screen.transform.localScale = Vector3.one;

        if (!transition_screen.activeSelf)
            transition_screen.SetActive(true);

        Invoke("RemoveTransitionScreen", 1.5f);
    }

    private void RemoveTransitionScreen()
    {
        if (transition_screen != null)
            Destroy(transition_screen);
    }

    public void RemoveLoadingScreen()
    {
        if (loading_screen != null)
            Destroy(loading_screen);
    }

    public void RemoveAllScreen()
    {
        CheckForActiveScreen();
        CheckForActivePopup();
        RemoveTransitionScreen();
        RemoveLoadingScreen();
    }

    public PopUpHandler LoadPopup(Popup popup)
    {
        CheckForActivePopup();

        GameObject temp = Resources.Load<GameObject>("UI/Popup/" + popup.ToString());
        active_popup = Instantiate(temp) as GameObject;
        active_popup.transform.SetParent(popup_content);

        RectTransform panel = (RectTransform)active_popup.transform;

        panel.transform.localPosition = Vector3.zero;
        panel.sizeDelta = Vector2.zero;

        active_popup.transform.localScale = Vector3.one;

        if (!active_popup.activeSelf)
            active_popup.SetActive(true);

        PopUpHandler _handler = active_popup.GetComponent<PopUpHandler>();

        return _handler;
    }

    void CheckForActiveScreen()
    {
        if (active_screen != null)
        {
            Destroy(active_screen);
            active_screen = null;
        }
    }

    public void CheckForActivePopup()
    {
        if (active_popup != null)
        {
            Destroy(active_popup);
            active_popup = null;
        }
    }

    void GotKeyPressed(MatKey _move)
    {
        if (active_popup)
        {
            OnHardwarePopup?.Invoke(_move);
        }
        else if (active_screen)
        {
            OnHardwareScreen?.Invoke(_move);
        }

        if(forceSendHardwareupdatetoScreen)
            OnHardwareScreen?.Invoke(_move);
    }
}
