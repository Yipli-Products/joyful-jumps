using UnityEngine;
using TMPro;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI actionNameText;

    public void ChangeToLeftTap()
    {
        actionNameText.text = "Left";
    }

    public void ChangeToRightTap()
    {
        actionNameText.text = "Right";
    }
}
