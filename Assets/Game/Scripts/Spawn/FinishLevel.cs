using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FinishLevel : MonoBehaviour
{

    public LayerMask playerLayer;

    private bool _isLevelEnded = false;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (playerLayer.Contains(other.gameObject))
        {
            if (!_isLevelEnded)
            {
                _isLevelEnded = true;

                Character character = other.GetComponent<Character>();

                if (character == null)
                {
                    return;
                }
                LevelManager.Instance.LevelSuccess(character);
            }
        }
    }
}
