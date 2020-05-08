using UnityEngine;

[CreateAssetMenu(fileName = "GameDataTracker", menuName = "Godspeed/GameDataTracker")]
public class GameDataTracker : ScriptableObject
{
    public int distanceCovered;
    public int timeElapsed;
    public int totalTime;

    public int totalPointsEarned;
}
