using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GodSpeedGames.Tools;

public class LoadingManager : PersistentSingleton<LoadingManager>
{

    public Animator loadingAnimation;
    public GameObject loader;

    public Animation _loadingBarAnimation;
    public GameObject gameOver;

    public GameObject fadeInFadeOut;
    public Animation aFadeInFadeOut;

    public GameDataTracker _tracker;

    private bool _gameOver;

    private void Start()
    {
        loader.SetActive(false);
        gameOver.gameObject.SetActive(false);
        fadeInFadeOut.SetActive(false);
    }

    public void LoadScreen(string scene, bool showGameOverPopup = false)
    {
        _gameOver = showGameOverPopup;
        StopCoroutine("Load");
        StartCoroutine("Load", scene);
    }

    IEnumerator Load(string scene)
    {
        InputController.Instance.DisableInput();

        loader.SetActive(true);

        loadingAnimation.SetTrigger("in");

        yield return new WaitForSeconds(1f);

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);

        yield return new WaitForSeconds(.5f);

        if (_gameOver)
        {
            gameOver.gameObject.SetActive(true);
            _loadingBarAnimation.Stop();
            _loadingBarAnimation.Play();
            // gameOver.SetData();
            yield return new WaitForSeconds(3f);
            gameOver.gameObject.SetActive(false);
        }

        loadingAnimation.SetTrigger("out");

        if (_gameOver)
        {
            LevelManager.Instance.ExcerciseCutScene();
            yield return new WaitForSeconds(4f);
        }
        else
        {
            _tracker.timeElapsed = 0;
            yield return new WaitForSeconds(1f);
        }

        loader.SetActive(false);

        if (_gameOver)
            LevelManager.Instance.StartTimer();

        //InputController.Instance.EnableInput();
    }

    public void ShowFadeInFadeOut() {
        fadeInFadeOut.SetActive(true);
        aFadeInFadeOut.Play();
        Invoke("HideFadeIn", 1.6f);
    }

    void HideFadeIn() {
        fadeInFadeOut.SetActive(false);
    }
}
