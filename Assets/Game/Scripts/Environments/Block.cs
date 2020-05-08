using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GodSpeedGames.Tools;

public class Block : MonoBehaviour
{

    public float targetTime = 2f;
    public Difficulty difficulty;
    [ReadOnly] public float lenght;

    [SerializeField] GameObject startPoint;
    [SerializeField] GameObject endPoint;

    protected virtual void Start()
    {
        lenght = EndingZPposition - StartingZPposition;
        lenght = Mathf.Abs(lenght);
    }

    public virtual float StartingZPposition {
        get {
            return startPoint.transform.position.z;
        }
    }

    public virtual float EndingZPposition
    {
        get
        {
            return endPoint.transform.position.z;
        }
    }
}
