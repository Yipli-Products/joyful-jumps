﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiDisplay : MonoBehaviour
{
    void Start()
    {
        if (!PlayerSession.Instance.currentYipliConfig.matPlayMode)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
