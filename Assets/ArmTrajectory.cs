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
    public Transform bottleHandParent = null;
    public Transform handRoot = null;


    bool inverseFront = false;
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
    private Vector3 offset;

    void Start()
    {
        // Keep a note of the time the movement started.
        startTime = Time.time;
        transform.rotation = neutral.rotation;
        transform.position = neutral.position;
        offset = bottleHandParent.position - handRoot.position;
        Debug.Log(Vector3.Distance(bottleHandParent.position, handRoot.position));
    }


    // Move to the target end position.
    void Update()
    {
        Debug.DrawLine(handRoot.position, bottleHandParent.position, Color.yellow);
        Debug.DrawLine(handRoot.position, handRoot.position + handRoot.transform.forward, Color.red);
        Vector3 normal = Vector3.Cross(bottleHandParent.position - handRoot.position, handRoot.position + handRoot.transform.forward);
        Debug.DrawLine(handRoot.position, handRoot.position + normal, Color.green);
        doArmMovement();
    }

    public void executeMovement(ArrayList movementSequence, bool inverseFront)
    {
        this.movementSequence = movementSequence;
        this.inverseFront = inverseFront;
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

    private void initCurrentMovement(Transform movementTarget)
    {
        if (!isCurrentMovementInitialized)
        {
            if (currentMovement == MovementType.FRONT)
            {
                if (inverseFront)
                {
                    offset = Quaternion.Euler(0, 90, -90) * offset;
                    movementTarget.Translate(offset);
                    movementTarget.Rotate(90f, 0, 0);
                }
                else
                {
                    offset = Quaternion.Euler(0, 90, 90) * offset;
                    movementTarget.Translate(offset);
                    movementTarget.Rotate(-90f, 0, 0);
                }
            }
            startTime = Time.time;
            journeyLength = Vector3.Distance(transform.position, movementTarget.position);
            startTrajectory = transform;
            endTrajectory = movementTarget;
        }
        else
        {
            isCurrentMovementInitialized = true;
        }
    }

    private void moveArmTo(Transform movementTarget)
    {
        if (isCurrentMovementInitialized)
        {
            if (transform.position == movementTarget.position)
            {
                isCurrentMovementInitialized = false;
            }
            doUpdate();
        }
        else
        {
            initCurrentMovement(movementTarget);
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
                inverseFront = false;
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
