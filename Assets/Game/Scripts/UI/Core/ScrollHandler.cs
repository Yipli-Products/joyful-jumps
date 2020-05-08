using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using GodSpeedGames.Tools;

public class ScrollHandler : MonoBehaviour
{

    public static Action OnSwipe;

    public enum Side
    {
        None,
        Right,
        Left
    }

    public RectTransform view;
    public RectTransform[] introImages;
    public bool decreseSizeWhenNotInMiddle = false;
    [Tooltip("Always there will be left and right to the middle slide")]
    public bool allowSlidesOnSides = false;

    private float wide;
    private float mousePositionStartX;
    private float mousePositionEndX;
    private float dragAmount;
    private float screenPosition;
    private float lastScreenPosition;
    private float lerpTimer;
    [SerializeField] private float lerpPage;

    public int pageCount = 1;
    public Side side = Side.None;

    public int swipeThrustHold = 30;
    public int spaceBetweenProfileImages = 400;
    public float lowestValue = 0.3f;
    public float higherValue = 0.651f;
    private bool canSwipe;

    bool startWelcomeScreen = false;

    [ReadOnly] public int actuallPageCount = 0;

    void Start()
    {
        //yield return new WaitForSeconds(.1f);
       // Init(0);
    }

    public void Init(int currentPageCount, bool delay = true)
    {
        if (delay)
            StartCoroutine("InitData", currentPageCount);
        else
            SetupPage(currentPageCount);
    }

    IEnumerator InitData(int currentPageCount) {
        yield return new WaitForSeconds(.05f);
        SetupPage(currentPageCount);
    }

    private void SetupPage(int currentPageCount) {
        if (allowSlidesOnSides)
        {
            if (currentPageCount == 0)
                currentPageCount = 1;
            else if (currentPageCount == introImages.Length - 1)
                currentPageCount = introImages.Length - 2;
        }
        wide = view.rect.width;
        pageCount = currentPageCount + 1;
        side = Side.Right;
        lerpPage = (wide + spaceBetweenProfileImages) * (pageCount - 1);
        lerpTimer = 0;
        OnSwipeComplete();

        startWelcomeScreen = true;
    }

    void Update()
    {
        if (!startWelcomeScreen)
            return;

        lerpTimer = lerpTimer + Time.deltaTime;
        if (lerpTimer < .333)
        {
            screenPosition = Mathf.Lerp(lastScreenPosition, lerpPage * -1, lerpTimer * 3);
            lastScreenPosition = screenPosition;
        }

        // Here we need to adjust drag area changing hardcore value.
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.y > (Screen.height * lowestValue) && Input.mousePosition.y < (Screen.height * higherValue))
        {
            canSwipe = true;
            mousePositionStartX = Input.mousePosition.x;
        }

        if (Input.GetMouseButton(0))
        {
            if (canSwipe)
            {
                mousePositionEndX = Input.mousePosition.x;
                dragAmount = mousePositionEndX - mousePositionStartX;

                if (allowSlidesOnSides)
                {
                    if ((actuallPageCount == 1 && dragAmount > 0) ||
                        ((actuallPageCount == introImages.Length - 2) && dragAmount < 0))
                    {
                        canSwipe = false;
                        return;
                    }

                }

                screenPosition = lastScreenPosition + dragAmount;
            }
        }

        if (Mathf.Abs(dragAmount) > swipeThrustHold && canSwipe)
        {
            canSwipe = false;
            lastScreenPosition = screenPosition;
            if (pageCount < introImages.Length)
                OnSwipeComplete();
            else if (pageCount == introImages.Length && dragAmount < 0)
                lerpTimer = 0;
            else if (pageCount == introImages.Length && dragAmount > 0)
                OnSwipeComplete();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Mathf.Abs(dragAmount) < swipeThrustHold)
            {
                lerpTimer = 0;
            }
        }

        for (int i = 0; i < introImages.Length; i++)
        {
            introImages[i].anchoredPosition = new Vector2(screenPosition + ((wide + spaceBetweenProfileImages) * i), 0);
            Image _image = introImages[i].GetComponent<Image>();

            if (decreseSizeWhenNotInMiddle)
            {

                if (side == Side.Right)
                {
                    if (i == pageCount - 1)
                    {
                        introImages[i].localScale = Vector3.Lerp(introImages[i].localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * 5);

                       /* if (_image != null)
                        {
                            Color temp = _image.color;
                            _image.color = new Color(temp.r, temp.g, temp.b, 1);
                        }*/
                    }
                    else
                    {
                        introImages[i].localScale = Vector3.Lerp(introImages[i].localScale, new Vector3(0.8f, 0.8f, 0.8f), Time.deltaTime * 5);

                       /* if (_image != null)
                        {
                            Color temp = _image.color;
                            _image.color = new Color(temp.r, temp.g, temp.b, 0.5f);
                        }*/
                    }
                }
                else
                {
                    if (i == pageCount)
                    {
                        introImages[i].localScale = Vector3.Lerp(introImages[i].localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * 5);

                       /* if (_image != null)
                        {
                            Color temp = _image.color;
                            _image.color = new Color(temp.r, temp.g, temp.b, 1);
                        }*/
                    }
                    else
                    {
                        introImages[i].localScale = Vector3.Lerp(introImages[i].localScale, new Vector3(0.8f, 0.8f, 0.8f), Time.deltaTime * 5);

                       /* if (_image != null)
                        {
                            Color temp = _image.color;
                            _image.color = new Color(temp.r, temp.g, temp.b, 0.5f);
                        }*/
                    }
                }
            }
        }
    }

    private void OnSwipeComplete()
    {
        lastScreenPosition = screenPosition;

        if (dragAmount > 0)
        {
            if (Mathf.Abs(dragAmount) > (swipeThrustHold))
            {
                if (pageCount == 0)
                {
                    lerpTimer = 0;
                    lerpPage = 0;
                }
                else
                {
                    if (side == Side.Right)
                        pageCount--;
                    side = Side.Left;
                    pageCount -= 1;
                    lerpTimer = 0;
                    if (pageCount < 0)
                        pageCount = 0;
                    lerpPage = (wide + spaceBetweenProfileImages) * pageCount;
                    //introimage[pagecount] is the current picture
                }

            }
            else
            {
                lerpTimer = 0;
            }

        }
        else if (dragAmount < 0)
        {
            if (Mathf.Abs(dragAmount) > (swipeThrustHold))
            {
                if (pageCount == introImages.Length)
                {
                    lerpTimer = 0;
                    lerpPage = (wide + spaceBetweenProfileImages) * introImages.Length - 1;
                }
                else
                {
                    if (side == Side.Left)
                        pageCount++;
                    side = Side.Right;
                    lerpTimer = 0;
                    lerpPage = (wide + spaceBetweenProfileImages) * pageCount;
                    pageCount++;
                    //introimage[pagecount] is the current picture
                }

            }
            else
            {
                lerpTimer = 0;
            }
        }

        if (side == Side.Right)
        {
            actuallPageCount = pageCount - 1;
        }
        else if (side == Side.Left)
        {
            actuallPageCount = pageCount;
        }

        OnSwipe?.Invoke();
    }
}
