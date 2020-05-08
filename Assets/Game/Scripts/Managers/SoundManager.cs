using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using GodSpeedGames.Tools;

public class SoundManager : PersistentSingleton<SoundManager>
{
    public enum Sound
    {
        ButtonSound,
        Error,
        UiTransition
    }

    public AudioClip buttonSound;
    public AudioClip errorSound;
    public AudioClip uiTransitionSound;

    public AudioMixer master;

    public AudioSource buttonSoundSource;

    private List<GameObject> _audioSource;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = new List<GameObject>();
    }

    public void PlaySound(Sound sound)
    {
        if (!PlayerData.Sound)
            return;

        switch (sound)
        {
            case Sound.ButtonSound:
                buttonSoundSource.PlayOneShot(buttonSound);
                break;
            case Sound.Error:
                buttonSoundSource.PlayOneShot(errorSound);
                break;
            case Sound.UiTransition:
                buttonSoundSource.PlayOneShot(uiTransitionSound);
                break;
        }
    }

    public AudioSource PlayMusic(GSGMusic music, Transform parent)
    {
        if (music.clip == null)
            return null;

        GameObject temp = new GameObject();
        temp.transform.SetParent(parent);
        temp.transform.localPosition = Vector3.zero;
        temp.transform.localRotation = Quaternion.identity;

       AudioSource audioSource = temp.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = music.mixerGroup;
        audioSource.spatialBlend = 1f;
        audioSource.clip = music.clip;


        if (PlayerData.Sound)
            audioSource.Play();

        if (music.duration > 0)
        {
            audioSource.loop = false;
            Destroy(temp, music.duration);
        }
        else
            audioSource.loop = true;

        _audioSource.Add(temp);

        return audioSource;
    }

    public void StopMusic()
    {
        for (int i = 0; i < _audioSource.Count; i++)
        {
            if (_audioSource[i] != null)
                Destroy(_audioSource[i]);
        }

        _audioSource.Clear();
    }

    public void PauseMusic(bool pause)
    {
        if (_audioSource == null)
            return;

        for (int i = 0; i < _audioSource.Count; i++)
        {
            if (_audioSource[i] != null)
            {
                if (pause)
                    _audioSource[i].GetComponent<AudioSource>().Pause();
                else
                    _audioSource[i].GetComponent<AudioSource>().UnPause();
            }
        }
    }

    public void TurnOffMusic()
    {
        for (int i = 0; i < _audioSource.Count; i++)
        {
            if (_audioSource[i] != null)
            {
                _audioSource[i].GetComponent<AudioSource>().Stop();
            }
        }

        PlayerData.Sound = false;
    }

    public void TurnOnMusic()
    {
        for (int i = 0; i < _audioSource.Count; i++)
        {
            if (_audioSource[i] != null)
            {
                _audioSource[i].GetComponent<AudioSource>().Play();
            }
        }

        PlayerData.Sound = true;
    }

    private void OnApplicationPause(bool pause)
    {
        PauseMusic(pause);
    }
}

[System.Serializable]
public struct GSGMusic
{
    public AudioMixerGroup mixerGroup;
    public AudioClip clip;
    public float duration;
}
