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

    private Vector3 sideOriginPos;

    private Quaternion sideOriginRot;

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
    private float ParabolaAnimation = 0;

    private Vector3 bottleForwardVector = new Vector3(1, 0, 0);
    private Vector3 bottleUpVector = new Vector3(0, 1, 0);
    private Vector3 bottleRightVector = new Vector3(0, 0, -1);

    public Transform bottle;

    public Transform bottleGrabAnchor;

    void Start()
    {
        // Keep a note of the time the movement started.
        transform.rotation = neutral.rotation;
        transform.position = neutral.position;
        frontOriginPos = front.position;
        frontOriginRot = front.rotation;
        sideOriginPos = side.position;
        sideOriginRot = side.rotation;
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

        if (Vector3.Distance(bottleGrabAnchor.position, bottleHandParent.position) < 0.01f)
        {
            bottle.parent = bottleHandParent.transform;
            bottle.GetComponent<Rigidbody>().isKinematic = true;

        }

        if (Vector3.Distance(transform.position, neutral.position) < 0.01f)
        {
            bottle.parent = null;
        }

        if (getPreviousMovement() == MovementType.SIDE && currentMovement == MovementType.FRONT)
        {
            if (Vector3.Distance(transform.position, front.position) < 0.01f)
            {
                bottle.parent = null;
                bottle.GetComponent<Rigidbody>().isKinematic = false;
                bottle.position = frontOriginPos;
            }
        }

        if (getPreviousMovement() == MovementType.FRONT && currentMovement == MovementType.SIDE)
        {
            Debug.Log(Vector3.Distance(transform.position, side.position));
            if (Vector3.Distance(transform.position, side.position) < 0.01f)
            {
                bottle.parent = null;
                bottle.GetComponent<Rigidbody>().isKinematic = false;
                bottle.position = sideOriginPos;
            }
        }
    }

    public void executeMovement(ArrayList movementSequence, bool inverseFront)
    {
        transform.rotation = neutral.rotation;
        transform.position = neutral.position;

        front.position = frontOriginPos;
        front.rotation = frontOriginRot;
        side.position = sideOriginPos;
        side.rotation = sideOriginRot;


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
        initCurrentMovement(neutral, getCurrentMovementTarget());
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
        if (inverseFront)
        {
            transform.rotation = Quaternion.Lerp(startTrajectory.rotation, endTrajectory.rotation, fractionOfJourney);

        }
    }

    private void initCurrentMovement(Transform movementStart, Transform movementTarget)
    {
        if (getPreviousMovement() == MovementType.NO_MOVEMENT && (currentMovement == MovementType.FRONT || currentMovement == MovementType.SIDE))
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
        Vector3 offset = bottleHandParent.position - handRoot.position;


        Vector3 handForwardVector = (handRoot.position + handRoot.transform.forward) - handRoot.position;
        Vector3 handUpVector = (handRoot.position + handRoot.transform.up) - handRoot.position;
        Vector3 handRightVector = (handRoot.position + handRoot.transform.right) - handRoot.position;

        Debug.Log(Quaternion.FromToRotation(handForwardVector, bottleForwardVector).eulerAngles);
        Debug.Log(Quaternion.FromToRotation(handRightVector, -bottleUpVector).eulerAngles);
        Debug.Log(Quaternion.FromToRotation(handUpVector, bottleRightVector).eulerAngles);


        if (inverseFront)
        {
            offset = Quaternion.Euler(90, 0, 0) * -offset;
            movementTarget.Translate(offset);
            movementTarget.Rotate(90f, 0, 0);
            getNextMovementTarget().Translate(offset);

            if (currentMovement == MovementType.FRONT)
            {
                side.Rotate(-90, 0, 0);
            }
            else
            {
                front.Rotate(-90, 0, 0);
            }
        }
        else
        {
            Quaternion frontx = Quaternion.FromToRotation(handForwardVector, bottleForwardVector);
            Quaternion right = Quaternion.FromToRotation(handRightVector, -bottleUpVector);
            Quaternion up = Quaternion.FromToRotation(handUpVector, bottleRightVector);

            offset = Quaternion.Euler(270, 0, 0) * -offset;
            movementTarget.Translate(offset);
            movementTarget.Rotate(270, 0, 0);
            getNextMovementTarget().Translate(offset);

            if (currentMovement == MovementType.FRONT)
            {
                side.Rotate(270f, 0, 0);
            }
            else
            {
                front.Rotate(270, 0, 0);
            }
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

    private Transform getCurrentMovementTarget()
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

    private Transform getNextMovementTarget()
    {
        if (currentMovementIndex >= movementSequence.Count - 1)
        {
            return null;
        }

        switch ((MovementType)movementSequence[currentMovementIndex + 1])
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
                    initCurrentMovement(front, getCurrentMovementTarget());
                }
            }

            if (currentMovement == MovementType.SIDE && Vector3.Distance(transform.position, side.position) < 0.01f)
            {
                currentMovementIndex += 1;
                if (currentMovementIndex < movementSequence.Count)
                {
                    currentMovement = (MovementType)movementSequence[currentMovementIndex];
                    initCurrentMovement(side, getCurrentMovementTarget());
                }
            }

            if (currentMovement == MovementType.NEUTRAL && Vector3.Distance(transform.position, neutral.position) < 0.01f)
            {
                currentMovementIndex += 1;
                if (currentMovementIndex < movementSequence.Count)
                {
                    currentMovement = (MovementType)movementSequence[currentMovementIndex];
                    initCurrentMovement(neutral, getCurrentMovementTarget());
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
