using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionScreen : MonoBehaviour
{
    public CharacterScriptable characterScriptable;
    public GameObject characterSelectionPanelPrefab;
    public GameObject characterModelPrefab;

    public Button nextArrow;
    public Button previousArrow;
    public Button selectButton;
    public TextMeshProUGUI buttonText;

    public Transform parent;

    private ScrollHandler _characterScroll;

    private List<GameObject> modelHolders;

    private int _totalLenght;
    private int _currentIndex = -1;

    private void OnEnable()
    {
        ScrollHandler.OnSwipe += UpdateButton;
    }

    private void OnDisable()
    {
        ScrollHandler.OnSwipe -= UpdateButton;
    }

    private void OnHardware(MatKey _action)
    {
        if (_action == MatKey.KeyRight)
        {
            ClickOnNextArrow();
        }
        else if (_action == MatKey.KeyLeft)
        {
            ClickOnPreviousArrow();
        }
    }

    void Start()
    {
        _characterScroll = GetComponent<ScrollHandler>();

        GameObject _characterPanel = null;
        GameObject _modelHolder = null;
        GameObject _characterModel = null;

       // _hardwareButton = gameObject.GetComponent<HardwareButtonSetup>();

        float _xPosition = 100f;
        _totalLenght = characterScriptable.characterData.Length;
        _characterScroll.introImages = new RectTransform[_totalLenght];

        if (modelHolders == null)
            modelHolders = new List<GameObject>();
        modelHolders.Clear();

        for (int i = 0; i < _totalLenght; i++)
        {
            _modelHolder = Instantiate<GameObject>(characterModelPrefab);
            _modelHolder.transform.position = Vector3.right * _xPosition;
            _xPosition += 5f;

            _characterModel = Instantiate<GameObject>(characterScriptable.characterData[i].playerPrefab);
            _characterModel.transform.SetParent(_modelHolder.transform, false);
            _modelHolder.GetComponentInChildren<Camera>().targetTexture = characterScriptable.characterData[i].renderTexture;

            _characterPanel = Instantiate<GameObject>(characterSelectionPanelPrefab);
            _characterPanel.transform.SetParent(parent, false);

            CharacterSelectionPanel _panel = _characterPanel.GetComponent<CharacterSelectionPanel>();
            _panel.SetCharacterData(characterScriptable.characterData[i].renderTexture, i);
            //_hardwareButton.AddButton(_panel.selectButton);

            _characterScroll.introImages[i] = _characterPanel.GetComponent<RectTransform>();

            modelHolders.Add(_modelHolder);
        }

       // _hardwareButton.SetCurrentButton(PlayerData.SelectedAvatarIndex);
        ValidateSwipe(PlayerData.SelectedAvatarIndex, true);
    }

    private void ValidateSwipe(int index, bool delay = false)
    {
        if (index <= 0)
        {
            index = 0;
            previousArrow.interactable = false;
            nextArrow.interactable = true;
        }
        else if (index >= _totalLenght - 1)
        {
            index = _totalLenght - 1;
            previousArrow.interactable = true;
            nextArrow.interactable = false;
        }
        else
        {
            previousArrow.interactable = true;
            nextArrow.interactable = true;
        }

        if (_currentIndex != index)
        {
            _currentIndex = index;
            _characterScroll.Init(_currentIndex, delay);
            UpdateButton();
        }
    }

    public void ClickOnNextArrow()
    {
        ValidateSwipe(_currentIndex + 1);
    }

    public void ClickOnPreviousArrow()
    {
        ValidateSwipe(_currentIndex - 1);
    }

    public void ClickOnSelect()
    {
        if (PlayerData.SelectedAvatarIndex != _characterScroll.actuallPageCount)
        {
            PlayerData.SelectedAvatarIndex = _characterScroll.actuallPageCount;

            selectButton.interactable = false;
            buttonText.text = "Selected";
        }
    }

    void UpdateButton()
    {
        if (PlayerData.SelectedAvatarIndex == _characterScroll.actuallPageCount)
        {
            selectButton.interactable = false;
            buttonText.text = "Selected";
        }
        else
        {
            selectButton.interactable = true;
            buttonText.text = "Select";
        }
    }

    public void ClickOnBack()
    {
        for (int i = 0; i < modelHolders.Count; i++)
            Destroy(modelHolders[i]);
        modelHolders.Clear();

        UiManager.Instance.LoadUI(UiManager.SCREEN.MenuScreen);
    }
}
