using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public YipliConfig currentYipliConfig;
    private void Awake()
    {
        currentYipliConfig.gameId = "joyfuljumps";
        //currentYipliConfig.currentGameType = YipliUtils.GameType.FITNESS_GAMING;
    }
}
