using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public static System.Action<bool> ShowJumpNotification;
    public static System.Action<bool> InsideJumpSafeArea;

    public float minRange;
    public float maxRange;

    public bool ignoreLeaveway = false;

    private GameObject player;
    private bool safeAreaZone = false;
    private bool jumpNotificstionAreaZone = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!ignoreLeaveway)
            CheckSafeAreaForPlayer();

        CheckJumpNotificationAreaForPlayer();
    }

    void CheckSafeAreaForPlayer()
    {
        float playerZPosition = player.transform.position.z;

        if ((playerZPosition >= (transform.position.z - maxRange)) && (playerZPosition <= (transform.position.z - minRange)))
        {
            if (!safeAreaZone)
            {
                // if player jumps from this zone, then this will be unsafe
                safeAreaZone = true;
                InsideJumpSafeArea?.Invoke(false);
            }
        }
        else
        {
            // outside of line
            if (safeAreaZone)
            {
                safeAreaZone = false;
                InsideJumpSafeArea?.Invoke(true);
            }
        }
    }

    void CheckJumpNotificationAreaForPlayer()
    {
        float playerZPosition = player.transform.position.z;

        if ((playerZPosition >= (transform.position.z - maxRange)) && (playerZPosition <= transform.position.z))
        {
            if (!jumpNotificstionAreaZone)
            {
                // if player jumps from this zone, then this will be unsafe
                jumpNotificstionAreaZone = true;
                ShowJumpNotification?.Invoke(true);
            }
        }
        else
        {
            // outside of line
            if (jumpNotificstionAreaZone)
            {
                jumpNotificstionAreaZone = false;
                ShowJumpNotification?.Invoke(false);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * .3f, transform.position + Vector3.up * .3f + Vector3.back * maxRange);
        Gizmos.DrawLine(transform.position + Vector3.up * .2f, transform.position + Vector3.up * .2f + Vector3.back * minRange);
    }
}
