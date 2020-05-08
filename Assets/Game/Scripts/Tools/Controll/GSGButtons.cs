using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GSGButtons : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, ISubmitHandler
{
    public static System.Action<ButtonType> OnGotActionFromButton;

    public enum ButtonType {
        None,
        Run,
        Jump
    }

    public ButtonType buttonType;
    public bool isContinuous = false;

    private bool _isKeyPressed = false;

    // Update is called once per frame
    void Update()
    {
        if (_isKeyPressed && isContinuous) {
            OnGotActionFromButton(buttonType);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isKeyPressed = true;
        if(!isContinuous)
            OnGotActionFromButton(buttonType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isKeyPressed = false;
        OnGotActionFromButton(ButtonType.None);
    }

    public void OnSubmit(BaseEventData eventData)
    {
       // throw new System.NotImplementedException();
    }
}
