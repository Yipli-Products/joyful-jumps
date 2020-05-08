using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class PopUpHandler : MonoBehaviour
{
    public TextMeshProUGUI titleTextPro;
    public Text titleText;

    protected Action Confirm;
    protected Action Deny;

    public virtual void OnConfirm()
    {
        Confirm?.Invoke();
        Destroy(gameObject);
    }

    public virtual void OnDeny()
    {
        Deny?.Invoke();
        Destroy(gameObject);
    }

    public virtual void OpenSingleButtonPopup(string title, Action _confirm)
    {
        SetTitle(title);
        Confirm = _confirm;
    }

    public virtual void OpenDoubleButtonPopup(string title, Action _confirm, Action _deny)
    {
        SetTitle(title);
        Confirm = _confirm;
        Deny = _deny;
    }

    public virtual void OpenDoubleButtonPopup(string title, Action<int> _confirm, Action _deny) { }

    void SetTitle(string title)
    {
        if (titleTextPro)
            titleTextPro.text = title;
        if (titleText)
            titleText.text = title;
    }
}
