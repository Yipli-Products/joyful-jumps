using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterSelectionPanel : MonoBehaviour
{
    public static System.Action OnAvatarSelect;

    public RawImage characteRawImage;
    public TextMeshProUGUI buttonText;
    public Button selectButton;

    private int _id;

    private void OnEnable()
    {
        OnAvatarSelect += UpdateButton;
    }

    private void OnDisable()
    {
        OnAvatarSelect -= UpdateButton;
    }

    public void SetCharacterData(RenderTexture texture, int id)
    {
        characteRawImage.texture = texture;
        _id = id;
        UpdateButton();
    }

    public void ClickOnSelect()
    {
        if (PlayerData.SelectedAvatarIndex != _id)
        {
            PlayerData.SelectedAvatarIndex = _id;
            OnAvatarSelect?.Invoke();
        }
    }

    void UpdateButton()
    {
        if (PlayerData.SelectedAvatarIndex == _id)
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
}
