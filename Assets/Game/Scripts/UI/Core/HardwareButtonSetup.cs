using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HardwareButtonSetup : MonoBehaviour
{
    public List<Button> buttons;
    public int defaultButtonIndex = 0;

    public bool isScrenType = true;

    [SerializeField] private int _currentButtonIndex = -1;

    private void OnEnable()
    {
        if (isScrenType)
            UiManager.OnHardwareScreen += OnHardware;
        else
            UiManager.OnHardwarePopup += OnHardware;
    }

    private void OnDisable()
    {
        if (isScrenType)
            UiManager.OnHardwareScreen -= OnHardware;
        else
            UiManager.OnHardwarePopup -= OnHardware;
    }

    private void Start()
    {
        if (_currentButtonIndex == -1)
            _currentButtonIndex = defaultButtonIndex;
        SelectCurrentButton(MatKey.None);
    }

    public void SetCurrentButton(int currentButton)
    {
        _currentButtonIndex = currentButton;
    }

    private void SelectCurrentButton(MatKey _action)
    {
        if (_action != MatKey.None && !buttons[_currentButtonIndex].interactable)
        {
            OnHardware(_action);
        }
        else
        {
            buttons[_currentButtonIndex].Select();
            
            if (buttons[_currentButtonIndex].GetComponent<Animator>())
            {
                /*if (buttons[_currentButtonIndex].name == "selectAvatar" || buttons[_currentButtonIndex].name == "changeProfile" || buttons[_currentButtonIndex].name == "GoToYipli" || buttons[_currentButtonIndex].name == "settings")
                {
                    buttons[_currentButtonIndex].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
                }
                else
                {
                    buttons[_currentButtonIndex].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
                }*/
                buttons[_currentButtonIndex].transform.GetChild(1).gameObject.SetActive(true);
                buttons[_currentButtonIndex].GetComponent<Animator>().enabled = true;
            }
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i != _currentButtonIndex)
            {
                if (buttons[i].GetComponent<Animator>())
                {
                    /*if (buttons[i].name == "selectAvatar" || buttons[i].name == "changeProfile" || buttons[i].name == "GoToYipli" || buttons[i].name == "settings")
                    {
                        buttons[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
                    }
                    else
                    {
                        buttons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = false;
                    }*/
                    buttons[i].transform.GetChild(1).gameObject.SetActive(false);
                    buttons[i].GetComponent<Animator>().enabled = false;
                    buttons[i].transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
    }

    private void ClickCurrentButton()
    {
        buttons[_currentButtonIndex].onClick.Invoke();
    }

    private void OnHardware(MatKey _action)
    {
        if (_action == MatKey.KeyRight)
        {
            _currentButtonIndex++;
            if (_currentButtonIndex > buttons.Count - 1)
                _currentButtonIndex = 0;

            SelectCurrentButton(_action);
        }
        else if (_action == MatKey.KeyLeft)
        {
            _currentButtonIndex--;
            if (_currentButtonIndex < 0)
                _currentButtonIndex = buttons.Count - 1;

            SelectCurrentButton(_action);
        }
        else if (_action == MatKey.KeyEnter)
        {
            ClickCurrentButton();
        }
    }

    public void AddButton(Button _button)
    {
        buttons.Add(_button);
    }
}
