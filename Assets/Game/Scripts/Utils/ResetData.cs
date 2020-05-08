using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetData : MonoBehaviour
{
    public GameDataTracker _tracker;
    void Start()
    {
        _tracker.distanceCovered = 0;
        _tracker.timeElapsed = 0;
        _tracker.totalTime = 0;
    }
}
