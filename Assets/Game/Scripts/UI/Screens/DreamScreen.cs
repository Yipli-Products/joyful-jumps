using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DreamScreen : MonoBehaviour
{
    public GameObject[] slides;

    public Button nextButton;
    public Button previousButton;

    private int currentSlide = 0;

    void Start()
    {
        UpdateSlide();
        previousButton.interactable = false;
        nextButton.interactable = true;
    }

    public void ClickOnNext()
    {
        currentSlide++;
        if (currentSlide >= slides.Length)
        {
            currentSlide = slides.Length - 1;
            LoadingManager.Instance.LoadScreen(Constant.GAME_SCENE_NAME);
        }
        else
        {
            previousButton.interactable = true;
            UpdateSlide();
        }
    }

    public void ClickOnPrevious()
    {
        currentSlide--;
        if (currentSlide < 0)
        {
            currentSlide = 0;
            previousButton.interactable = false;
        }
        else
        {
            if (currentSlide == 0)
                previousButton.interactable = false;
            UpdateSlide();
        }
    }

    void UpdateSlide()
    {
        for (int i = 0; i < slides.Length; i++)
        {
            if (i == currentSlide)
                slides[i].SetActive(true);
            else
                slides[i].SetActive(false);
        }
    }
}
