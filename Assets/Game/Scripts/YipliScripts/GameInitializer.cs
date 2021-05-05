using System;
using UnityEngine;
using Firebase.DynamicLinks;

public class GameInitializer : MonoBehaviour
{
    public YipliConfig currentYipliConfig;

    private void Awake()
    {
        currentYipliConfig.gameId = "joyfuljumps";
        currentYipliConfig.gameType = GameType.FITNESS_GAMING;
    }  
}
