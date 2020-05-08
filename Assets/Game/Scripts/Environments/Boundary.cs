using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    public LayerMask ignoredObjetLayer;

    protected virtual void OnTriggerEnter(Collider collider)
    {

        if (!ignoredObjetLayer.Contains(collider.gameObject)) {
            collider.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!ignoredObjetLayer.Contains(collision.gameObject))
        {
            collision.gameObject.SetActive(false);
        }
    }
}
