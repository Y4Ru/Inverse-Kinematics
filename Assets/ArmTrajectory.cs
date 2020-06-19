using UnityEngine;
using System.Collections;

public class ArmTrajectory : MonoBehaviour
{
    // Transforms to act as start and end markers for the journey.
    public Transform neutral;
    public Transform front;
    public Transform side;

    // Movement duration from start to end marker.
    public float movementDuration = 1.0F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    private bool directionChange;

    private Transform startTrajectory = null;
    private Transform endTrajectory = null;

    private bool isCurrentMovementInitialized = false;

    private MovementType currentMovement = MovementType.NO_MOVEMENT;

    void Start()
    {
        // Keep a note of the time the movement started.
        startTime = Time.time;
        transform.rotation = neutral.rotation;
        transform.position = neutral.position;
    }

    // Move to the target end position.
    void Update()
    {
        doArmMovement();
    }

    void doUpdate()
    {
        //startTime = Time.time;

        float dynamicSpeed = journeyLength / movementDuration;

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * dynamicSpeed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startTrajectory.position, endTrajectory.position, fractionOfJourney);

        transform.rotation = Quaternion.Lerp(startTrajectory.rotation, endTrajectory.rotation, fractionOfJourney);

    }

    private void initCurrentMovement(Transform movementTargetPosition)
    {
        // init Movement
        if (!isCurrentMovementInitialized)
        {
            startTime = Time.time;
            journeyLength = Vector3.Distance(transform.position, movementTargetPosition.position);
            startTrajectory = transform;
            endTrajectory = movementTargetPosition;
        }
        else
        {
            isCurrentMovementInitialized = true;
        }
    }

    private void moveArmTo(Transform movementTargetPosition)
    {
        Debug.Log(movementTargetPosition.name);
        Debug.Log(isCurrentMovementInitialized);
        if (isCurrentMovementInitialized)
        {
            doUpdate();
            if (transform.position == movementTargetPosition.position)
            {
                isCurrentMovementInitialized = false;
            }
        }
        else
        {
            initCurrentMovement(movementTargetPosition);
            isCurrentMovementInitialized = true;
        }
    }

    public void doArmMovement()
    {
        if (transform.position == neutral.position)
        {
            currentMovement = MovementType.FRONT;
        }
        else if (transform.position == front.position)
        {
            currentMovement = MovementType.SIDE;
        }
        else if (transform.position == side.position)
        {
            currentMovement = MovementType.NEUTRAL;
        }

        switch (currentMovement)
        {
            case MovementType.FRONT:
                moveArmTo(front);
                break;
            case MovementType.SIDE:
                moveArmTo(side);
                break;
            case MovementType.NEUTRAL:
                moveArmTo(neutral);
                break;
            default:
                break;
        }
    }

    private enum MovementType
    {
        NO_MOVEMENT,
        FRONT,
        SIDE,
        NEUTRAL
    }
}
