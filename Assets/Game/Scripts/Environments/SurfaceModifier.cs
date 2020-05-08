using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GodSpeedGames.Tools;

public class SurfaceModifier : MonoBehaviour
{
    [Tooltip("Set the player layer")]
    public LayerMask layers;

    [Header("Friction")]
    [Tooltip("Set a friction between 0.01 and 0.99 to get a slippery surface (close to 0 is very slippery, close to 1 is less slippery).\nOr set it above 1 to get a sticky surface. The higher the value, the stickier the surface. Set 0 for no friction")]
    public float Friction = 0;

    [Header("Force")]
    [Tooltip("Use these to add Z or Y (or both) forces to any Character Controller that gets grounded on this surface. Adding a X force will create a treadmill (negative value > treadmill to the left, positive value > treadmill to the right). A positive y value will create a trampoline, a bouncy surface, or a jumper for example.")]
    public Vector2 AddedForce = Vector2.zero;

    [ReadOnly] public Character _character;

    protected virtual void OnTriggerStay(Collider collider)
    {
        if (layers.Contains(collider.gameObject))
        {
            _character = collider.gameObject.GetComponentNoAlloc<Character>();
            _character.AddFriction(Friction);
        }
    }

    protected virtual void OnTriggerExit(Collider collider)
    {
        if (layers.Contains(collider.gameObject))
        {
            if (_character != null)
                _character.AddFriction(0);
            _character = null;
        }
    }

    protected virtual void Update()
    {
        if (_character == null)
        {
            return;
        }

        if (AddedForce.x != 0)
        {
            Vector3 movement = Vector3.zero;
            movement.z = AddedForce.x * Time.deltaTime;
            _character.Move(movement);
        }

        if (AddedForce.y != 0)
        {
            _character.gameObject.GetComponentNoAlloc<CharacterVerticalMovement>().SetVerticleSpeed(AddedForce.y);
        }
    }
}
