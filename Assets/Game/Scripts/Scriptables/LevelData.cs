using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Godspeed/LevelData")]
public class LevelData : ScriptableObject
{
    public LevelDifficulty[] levelInfo;
}

[System.Serializable]
public struct LevelDifficulty
{
    public Difficulty difficulty;
    public float lenght;
}
