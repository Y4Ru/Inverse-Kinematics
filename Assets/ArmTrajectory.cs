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

    private Transform startTrajectory = null;
    private Transform endTrajectory = null;

    private bool isCurrentMovementInitialized = false;

    private MovementType currentMovement;

    private ArrayList movementSequence = null;

    private int currentMovementIndex = 0;
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

    public void executeMovement(ArrayList sequence)
    {
        movementSequence = sequence;
    }

    private void doUpdate()
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
        Debug.Log(currentMovementIndex);
        if (isCurrentMovementInitialized)
        {
            if (transform.position == movementTargetPosition.position)
            {
                isCurrentMovementInitialized = false;
            }
            doUpdate();
        }
        else
        {
            initCurrentMovement(movementTargetPosition);
            isCurrentMovementInitialized = true;
        }
    }

    private void doArmMovement()
    {

        if (movementSequence != null && movementSequence.Count > 0)
        {
            currentMovement = (MovementType)movementSequence[currentMovementIndex];

            if (currentMovement == MovementType.FRONT && transform.position == front.position)
            {
                currentMovementIndex += 1;
            }

            if (currentMovement == MovementType.SIDE && transform.position == side.position)
            {
                currentMovementIndex += 1;
            }

            if (currentMovement == MovementType.NEUTRAL && transform.position == neutral.position)
            {
                currentMovementIndex += 1;
            }

            if (currentMovementIndex >= movementSequence.Count)
            {
                currentMovement = MovementType.NO_MOVEMENT;
                currentMovementIndex = 0;
                movementSequence = null;
            }
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
            case MovementType.NO_MOVEMENT:
                break;
            default:
                break;
        }
    }
}
