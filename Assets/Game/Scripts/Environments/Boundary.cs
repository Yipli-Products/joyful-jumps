using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    public LayerMask ignoredObjetLayer;

    protected virtual void OnTriggerEnter(Collider collider)
    {
        LevelManager.Instance.isPlayerDiedByFalling = true;

        if (!ignoredObjetLayer.Contains(collider.gameObject)) {
            collider.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        LevelManager.Instance.isPlayerDiedByFalling = true;

        if (!ignoredObjetLayer.Contains(collision.gameObject))
        {
            collision.gameObject.SetActive(false);
        }
    }
}
