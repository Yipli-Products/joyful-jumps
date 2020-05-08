using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GodSpeedGames.Tools;


/// <summary>
/// Checkpoint class. Will make the player respawn at this point if it dies.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class CheckPoint : MonoBehaviour
{

    protected bool _reached = false;


    /// <summary>
    /// Spawns the player at the checkpoint.
    /// </summary>
    /// <param name="player">Player.</param>
    public virtual void SpawnPlayer(Character player)
    {
        player.RespawnAt(transform);
    }

    protected virtual void OnTriggerEnter(Collider collider)
    {
        Character character = collider.GetComponent<Character>();

        if (character == null) { return; }
        if (_reached) { return; }
        if (LevelManager.Instance == null) { return; }

        LevelManager.Instance.SetCurrentCheckpoint(this);
    }


    /// <summary>
    /// On DrawGizmos, we draw lines to show the path the object will follow
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
#if UNITY_EDITOR

        if (LevelManager.Instance == null)
        {
            return;
        }

        if (LevelManager.Instance.Checkpoints == null)
        {
            return;
        }

        if (LevelManager.Instance.Checkpoints.Count == 0)
        {
            return;
        }

        for (int i = 0; i < LevelManager.Instance.Checkpoints.Count; i++)
        {
            // we draw a line towards the next point in the path
            if ((i + 1) < LevelManager.Instance.Checkpoints.Count)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(LevelManager.Instance.Checkpoints[i].transform.position, LevelManager.Instance.Checkpoints[i + 1].transform.position);
            }
        }
#endif
    }
}
