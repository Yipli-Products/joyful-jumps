using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubuaLogic : MonoBehaviour
{
    public string reponse = "1.Running+2+1.3";
    public bool doEdit = false;

    // Update is called once per frame
    void Update()
    {
        if (doEdit)
        {
            ParsePlease();
            doEdit = false;
        }
    }

    void ParsePlease()
    {
        string[] FMTokens = reponse.Split('.');

        Debug.Log("count " + FMTokens[0]);

        if (FMTokens.Length > 1) {
            string[] whiteSpace = FMTokens[1].Split('+');

            Debug.Log("status " + whiteSpace[0]);

            if (whiteSpace.Length > 1)
            {
                Debug.Log("step count " + whiteSpace[1]);
            }
        }
    }
}
