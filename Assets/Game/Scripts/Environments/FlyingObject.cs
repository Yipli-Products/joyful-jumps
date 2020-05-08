using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObject : MonoBehaviour
{
    const float k_OffScreenError = .1f;

    public float speed = 10;
    [Tooltip("Original Sprite Face Direction")]
    public bool isFacingRight = false;

    [Header("Screen Position")]
    public float minForwardZPosition;
    public float maxForwardZPosition;

    public float minYPosition;
    public float maxYPosition;

    public float rightXPosition = 40f;

    private Camera mainCamera;
    private SpriteRenderer spRenderer;

    private Vector3 direction;

    private bool isEnabled = false;

    void Awake()
    {
        mainCamera = Camera.main;
        spRenderer = GetComponent<SpriteRenderer>();
        if (spRenderer == null)
            spRenderer = GetComponentInChildren<SpriteRenderer>();
        isEnabled = true;
    }

    private void Start()
    {
        spRenderer.color = ColorPalletManager.Instance.SunLight;
    }

    public void SetDirection(Vector3 rootPosition)
    {

        float z = Random.Range(minForwardZPosition, maxForwardZPosition) + rootPosition.z;
        float y = Random.Range(minYPosition, maxYPosition);

        float x;

        float random = Random.Range(0f, 1f);
        if (random > .5)
        {
            direction = Vector3.left;
            x = rightXPosition;
            spRenderer.flipX = isFacingRight;
        }
        else
        {
            direction = Vector3.right;
            x = -rightXPosition;
            spRenderer.flipX = !isFacingRight;
        }

        transform.position = new Vector3(x, y, z);

        isEnabled = true;
    }

    void Update()
    {
        if (isEnabled)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > -k_OffScreenError &&
                            screenPoint.x < 1 + k_OffScreenError && screenPoint.y > -k_OffScreenError &&
                            screenPoint.y < 1 + k_OffScreenError;

            if (onScreen)
            {
                transform.Translate(direction.normalized * Time.deltaTime * speed);
            }
            else
            {
                isEnabled = false;
                gameObject.SetActive(false);
            }
        }
    }
}
