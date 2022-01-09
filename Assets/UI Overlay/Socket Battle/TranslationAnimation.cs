using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Moves the game object to the given position at the given speed
/// </summary>
public class TranslationAnimation : MonoBehaviour
{
    public float animTargetPositionX;
    public float animTargetPositionY;
    public float animTargetPositionZ;

    /// <summary>
    /// Where the game object is moving to
    /// </summary>
    private Vector3 targetPosition
    {
        get
        {
            return new Vector3(animTargetPositionX, animTargetPositionY, animTargetPositionZ);
        }
        set
        {
            animTargetPositionX = value.x;
            animTargetPositionY = value.y;
            animTargetPositionZ = value.z;
        }
    }

    /// <summary>
    /// How fast the game object should move to the location
    /// </summary>
    public float distancePerSecond;

    /// <summary>
    /// Disable to prevent interactions when transitioning
    /// </summary>
    public EventTrigger targetEventTrigger;

    /// <summary>
    /// The game object to translate
    /// </summary>
    public GameObject _target;

    /// <summary>
    /// The game object to translate
    /// </summary>
    public GameObject target
    {
        get { return (_target != null) ? _target : gameObject; }
        set { _target = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (distancePerSecond < 0) { throw new System.Exception("WARN: Negative speed provided, will never reach target position."); }

        // Get the direction (normal) of the movement
        var movementDirection = targetPosition - target.transform.position;
        movementDirection.Normalize();

        // Apply the speed to determine next location
        var movement = movementDirection * distancePerSecond * Time.deltaTime;
        var nextPosition = target.transform.position + movement;

        // Check if translation stepped over target and cap it's movement
        var isPastTarget = TranslationPastTarget(nextPosition, movementDirection);
        if (isPastTarget)
        {
            nextPosition = targetPosition;
            target.transform.position = nextPosition;

            // Stop the update from running so movement controls are no longer
            enabled = false;
            if (targetEventTrigger) { targetEventTrigger.enabled = true; }
        } else
        {
            target.transform.position = nextPosition;
        }
    }

    /// <summary>
    /// Script version to kick this over.  These params can also be set in the animator
    /// </summary>
    /// <param name="_targetPosition"></param>
    /// <param name="_distancePerSecond"></param>
    public void Translate(Vector3 _targetPosition, float _distancePerSecond)
    {
        targetPosition = _targetPosition;
        distancePerSecond = _distancePerSecond;
        enabled = true;
        if (targetEventTrigger) { targetEventTrigger.enabled = false; }
    }

    /// <summary>
    /// Return true if the next position went past the target location
    /// </summary>
    /// <param name="nextPosition"></param>
    /// <param name="movementDirection">Assuming this is a normal vector</param>
    /// <returns></returns>
    private bool TranslationPastTarget(Vector3 nextPosition, Vector3 movementDirection)
    {
        // Get next movement direction
        var nextMovementDirection = targetPosition - nextPosition;
        nextMovementDirection.Normalize();

        var nextDirectionIsTheSame = (
            AreSameSign(nextMovementDirection.x, movementDirection.x) &&
            AreSameSign(nextMovementDirection.y, movementDirection.y) &&
            AreSameSign(nextMovementDirection.z, movementDirection.z)
        );
        return !nextDirectionIsTheSame;
    }

    /// <summary>
    /// Returns true if the left and right are the same sign. If the number is too small true is returned until a more definitive conclusion can be drawn.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    private bool AreSameSign(float left, float right)
    {
        if (left > 0.05f && right > 0.05f) { return true; }
        if (left < -0.05f && right < -0.05f) { return true; }
        if (left > 0.05f && right < -0.05f) { return false; }
        if (left < -0.05f && right > 0.05f) { return false; }

        // If the conditions didn't meet the threathold return true
        return true;
    }
}
