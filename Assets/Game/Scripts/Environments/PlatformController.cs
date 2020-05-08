using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Vector3[] localWaypoints;
    [Tooltip("Set the player layer")]
    public LayerMask layers;

    public float speed;
    public bool cyclic;
    [Tooltip("Wait time before moving")]
    public float waitTime;
    [Range(0, 2)]
    public float easeAmount;

    private Vector3[] globalWaypoints;

    private int fromWaypointIndex;

    private float percentBetweenWaypoints;
    private float nextMoveTime;

    private Character _jumpedCharacter;

    protected virtual void Start()
    {
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    protected virtual float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    protected virtual Vector3 CalculatePlatformMovement()
    {

        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return (newPos - transform.position);
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }


    protected virtual void FixedUpdate()
    {
        Vector3 velocity = CalculatePlatformMovement();
        transform.position += velocity;

        if (_jumpedCharacter != null)
            _jumpedCharacter.Move(velocity);
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (layers.Contains(other.gameObject))
        {
            if (_jumpedCharacter == null)
                _jumpedCharacter = other.gameObject.GetComponentNoAlloc<Character>();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (layers.Contains(other.gameObject))
        {
            _jumpedCharacter = null;
        }
    }
}
