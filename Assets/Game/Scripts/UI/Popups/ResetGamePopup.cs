using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGamePopup : MonoBehaviour
{
    public static System.Action OnGameReset;

    public void OnReset() {
        PlayerPrefs.DeleteAll();
        SoundManager.Instance.TurnOnMusic();
        OnGameReset?.Invoke();
        Destroy(gameObject);
    }

    public void Cancel() {
        Destroy(gameObject);
    }
}
