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

    private Vector3 frontOriginPos;

    private Quaternion frontOriginRot;

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

    private float ParabolaAnimation = 0;

    private Vector3 bottleForwardVector = new Vector3(1, 0, 0);
    private Vector3 bottleUpVector = new Vector3(0, 1, 0);
    private Vector3 bottleRightVector = new Vector3(0, 0, -1);

    void Start()
    {
        // Keep a note of the time the movement started.
        transform.rotation = neutral.rotation;
        transform.position = neutral.position;
        offset = bottleHandParent.position - handRoot.position;
        frontOriginPos = front.position;
        frontOriginRot = front.rotation;
    }


    // Move to the target end position.
    void Update()
    {
        //Debug.DrawLine(handRoot.position, bottleHandParent.position, Color.yellow);
        Debug.DrawLine(handRoot.position, handRoot.position + handRoot.transform.up, Color.yellow);
        Debug.DrawLine(handRoot.position, handRoot.position + handRoot.transform.forward, Color.red);
        Vector3 normal = Vector3.Cross(bottleHandParent.position - handRoot.position, handRoot.position + handRoot.transform.forward);
        //Debug.DrawLine(handRoot.position, handRoot.position + normal, Color.green);
        Debug.DrawLine(handRoot.position, handRoot.position + handRoot.transform.right, Color.green);
        doArmMovement();
    }

    public void executeMovement(ArrayList movementSequence, bool inverseFront)
    {
        transform.rotation = neutral.rotation;
        transform.position = neutral.position;

        front.position = frontOriginPos;
        front.rotation = frontOriginRot;


        startTrajectory = null;
        endTrajectory = null;
        isCurrentMovementInitialized = false;
        ParabolaAnimation = 0;

        this.movementSequence = movementSequence;
        this.inverseFront = inverseFront;
        currentMovementIndex = 0;
        if (movementSequence != null && movementSequence.Count > 0)
        {
            currentMovement = (MovementType)movementSequence[currentMovementIndex];
        }
        initCurrentMovement(neutral, getMovementTarget());
    }

    private void doLinearUpdate()
    {
        float dynamicSpeed = journeyLength / movementDuration;

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * dynamicSpeed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startTrajectory.position, endTrajectory.position, fractionOfJourney);
        transform.rotation = Quaternion.Lerp(startTrajectory.rotation, endTrajectory.rotation, fractionOfJourney);
    }

    private void doParabolaUpdate()
    {
        float dynamicSpeed = journeyLength / movementDuration;

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * dynamicSpeed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = MathParabola.Parabola(startTrajectory.position, endTrajectory.position, 0.5f, fractionOfJourney);
        transform.rotation = Quaternion.Lerp(startTrajectory.rotation, endTrajectory.rotation, fractionOfJourney);
    }

    private void initCurrentMovement(Transform movementStart, Transform movementTarget)
    {
        if (currentMovement == MovementType.FRONT)
        {
            initOffset(movementTarget);
        }
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, movementTarget.position);
        startTrajectory = movementStart;
        endTrajectory = movementTarget;

        isCurrentMovementInitialized = true;
    }

    private void initOffset(Transform movementTarget)
    {
        offset = bottleHandParent.position - handRoot.position;


        Vector3 handFrontVector = (handRoot.position + handRoot.transform.forward) - handRoot.position;
        Vector3 handUpVector = (handRoot.position + handRoot.transform.up) - handRoot.position;
        Vector3 handRightVector = (handRoot.position + handRoot.transform.right) - handRoot.position;

        Debug.Log(offset);
        Debug.Log(handFrontVector);

        Debug.Log(Quaternion.FromToRotation(handFrontVector, bottleForwardVector).eulerAngles);

        if (inverseFront)
        {
            offset = Quaternion.Euler(90, 0, 0) * -offset;
            movementTarget.Translate(offset);
            movementTarget.Rotate(90f, 0, 0);
        }
        else
        {
            offset = Quaternion.Euler(-90, 0, 0) * -offset;
            movementTarget.Translate(offset);
            movementTarget.Rotate(-90f, 0, 0);
        }
    }

    private Transform getMovementOrigin()
    {
        switch (getPreviousMovement())
        {
            case MovementType.FRONT:
                return front;
            case MovementType.SIDE:
                return side;
            case MovementType.NEUTRAL:
                return neutral;
            case MovementType.NO_MOVEMENT:
                return null;
            default:
                return null;
        }
    }

    private void moveArmTo(Transform movementTarget)
    {
        //Debug.Log(isCurrentMovementInitialized);
        //Debug.Log(currentMovement);
        //Debug.Log(Vector3.Distance(transform.position, movementTarget.position));

        if (Vector3.Distance(transform.position, movementTarget.position) < 0.001f)
        {
            isCurrentMovementInitialized = false;
        }
        if (currentMovement == MovementType.SIDE && getPreviousMovement() == MovementType.FRONT || currentMovement == MovementType.FRONT && getPreviousMovement() == MovementType.SIDE)
        {
            doParabolaUpdate();
        }
        else
        {
            doLinearUpdate();

        }


    }

    private MovementType getPreviousMovement()
    {
        if (currentMovementIndex == 0)
        {
            return MovementType.NO_MOVEMENT;
        }

        return (MovementType)movementSequence[currentMovementIndex - 1];
    }

    private Transform getMovementTarget()
    {
        switch (currentMovement)
        {
            case MovementType.FRONT:
                return front;
            case MovementType.SIDE:
                return side;
            case MovementType.NEUTRAL:
                return neutral;
            case MovementType.NO_MOVEMENT:
                return null;
            default:
                return null;
        }

    }
    private void doArmMovement()
    {
        if (movementSequence != null && movementSequence.Count > 0)
        {

            if (currentMovement == MovementType.FRONT && Vector3.Distance(transform.position, front.position) < 0.01f)
            {
                currentMovementIndex += 1;
                if (currentMovementIndex < movementSequence.Count)
                {
                    currentMovement = (MovementType)movementSequence[currentMovementIndex];
                    initCurrentMovement(front, getMovementTarget());
                }
            }

            if (currentMovement == MovementType.SIDE && Vector3.Distance(transform.position, side.position) < 0.01f)
            {
                currentMovementIndex += 1;
                if (currentMovementIndex < movementSequence.Count)
                {
                    currentMovement = (MovementType)movementSequence[currentMovementIndex];
                    initCurrentMovement(side, getMovementTarget());
                }
            }

            if (currentMovement == MovementType.NEUTRAL && Vector3.Distance(transform.position, neutral.position) < 0.01f)
            {
                currentMovementIndex += 1;
                if (currentMovementIndex < movementSequence.Count)
                {
                    currentMovement = (MovementType)movementSequence[currentMovementIndex];
                    initCurrentMovement(neutral, getMovementTarget());
                }
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
