using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBlock : MonoBehaviour
{

    public enum AnimParameter {
        none,
        walk,
        jump
    }

    public static System.Action<string, string> OnShowTutorialMsg; // msg, anim parameter

    [Tooltip("Set the player layer")]
    public LayerMask playerLayer;

    [TextArea]
    public string tutorialMsg;
    public AnimParameter animParameter;

    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerLayer.Contains(other.gameObject))
        {
            OnShowTutorialMsg?.Invoke(tutorialMsg, animParameter.ToString());
        }
    }
}
