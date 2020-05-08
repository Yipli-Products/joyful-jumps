using System.Collections;
using System.Collections.Generic;
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
            OnHardware(_action);
        else
            buttons[_currentButtonIndex].Select();
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
