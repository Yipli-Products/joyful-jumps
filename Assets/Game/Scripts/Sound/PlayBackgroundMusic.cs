using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBackgroundMusic : MonoBehaviour
{
    public GSGMusic backgroundMusic;

    void Start()
    {
        SoundManager.Instance.PlayMusic(backgroundMusic, Camera.main.transform);
    }
}
