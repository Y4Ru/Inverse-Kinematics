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

    bool underhandFront = false;
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

    private bool bottleGrabbed = false;

    private float bottleDetectionDist = 0.1f;
    private GripAnimator fingerAnimator;
    public Transform fingerTarget;

    public MovementType GetCurrentMovementType()
    {
        return currentMovement;
    }

    void Start()
    {
        // Keep a note of the time the movement started.
        transform.rotation = neutral.rotation;
        transform.position = neutral.position;
        frontOriginPos = front.position;
        frontOriginRot = front.rotation;
        sideOriginPos = side.position;
        sideOriginRot = side.rotation;
        setHandOffsetPositions();
        fingerAnimator = fingerTarget.GetComponent<GripAnimator>();

    }


    // Move to the target end position.
    void Update()
    {
        //Debug.DrawLine(handRoot.position, bottleHandParent.position, Color.yellow);
        //Debug.DrawLine(handRoot.position, handRoot.position + handRoot.transform.up, Color.yellow);
        //Debug.DrawLine(handRoot.position, handRoot.position + handRoot.transform.forward, Color.red);
        //Vector3 normal = Vector3.Cross(bottleHandParent.position - handRoot.position, handRoot.position + handRoot.transform.forward);
        //Debug.DrawLine(handRoot.position, handRoot.position + normal, Color.green);
        //Debug.DrawLine(handRoot.position, handRoot.position + handRoot.transform.right, Color.green);
        doArmMovement();
    }

    private void detectFrontBottleGrab()
    {
        if (currentMovement == MovementType.FRONT && getPreviousMovement() != MovementType.SIDE && Vector3.Distance(bottleGrabAnchor.position, bottleHandParent.position) < bottleDetectionDist)
        {
            bottle.parent = bottleHandParent.transform;
            bottle.GetComponent<Rigidbody>().isKinematic = true;
            bottleGrabbed = true;
        }
    }

    private void detectSideBottleGrab()
    {
        if ((currentMovement == MovementType.SIDE && getPreviousMovement() != MovementType.FRONT && Vector3.Distance(bottleGrabAnchor.position, bottleHandParent.position) < bottleDetectionDist))
        {
            bottle.parent = bottleHandParent.transform;
            bottle.GetComponent<Rigidbody>().isKinematic = true;
            bottleGrabbed = true;
        }
    }

    private void detectFrontBottleDrop()
    {
        if (getPreviousMovement() == MovementType.SIDE && currentMovement == MovementType.FRONT)
        {
            bottle.parent = null;
            bottle.GetComponent<Rigidbody>().isKinematic = false;
            bottle.position = frontOriginPos;
            bottleGrabbed = false;
        }
    }

    private void detectSideBottleDrop()
    {
        if (getPreviousMovement() == MovementType.FRONT && currentMovement == MovementType.SIDE)
        {
            bottle.parent = null;
            bottle.GetComponent<Rigidbody>().isKinematic = false;
            bottle.position = sideOriginPos;
            bottleGrabbed = false;
        }
    }

    public void executeMovement(ArrayList movementSequence, bool underhandFront)
    {
        transform.rotation = neutral.rotation;
        transform.position = neutral.position;

        front.position = frontOriginPos;
        front.rotation = frontOriginRot;
        side.position = sideOriginPos;
        side.rotation = sideOriginRot;

        Debug.Log(side.position);



        startTrajectory = null;
        endTrajectory = null;
        isCurrentMovementInitialized = false;
        ParabolaAnimation = 0;

        this.movementSequence = movementSequence;
        this.underhandFront = underhandFront;
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

    private void doParabolaUpdate(string direction)
    {
        float dynamicSpeed = journeyLength / movementDuration;

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * dynamicSpeed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = MathParabola.Parabola(startTrajectory.position, endTrajectory.position, 0.5f, fractionOfJourney, direction);
        transform.rotation = Quaternion.Lerp(startTrajectory.rotation, endTrajectory.rotation, fractionOfJourney);
    }

    private void initCurrentMovement(Transform movementStart, Transform movementTarget)
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, movementTarget.position);
        startTrajectory = movementStart;
        endTrajectory = movementTarget;

        isCurrentMovementInitialized = true;
    }

    private void setHandOffsetPositions()
    {
        Vector3 offset = bottleHandParent.position - handRoot.position;

        Transform frontunderhandTransform = front.GetChild(0);
        Transform frontOverhandTransform = front.GetChild(1);
        Transform sideunderhandTransform = side.GetChild(0);
        Transform sideOverhandTransform = side.GetChild(1);

        setUnderhandOffset(frontunderhandTransform);
        setOverhandOffset(frontOverhandTransform, 30);
        setUnderhandOffset(sideunderhandTransform);
        setOverhandOffset(sideOverhandTransform, 75);
    }

    private void setUnderhandOffset(Transform underhandTransform)
    {
        Vector3 offset = bottleHandParent.position - handRoot.position;

        offset = Quaternion.Euler(0, 90, -90) * offset;
        underhandTransform.Translate(offset);
        underhandTransform.Rotate(90, 0, 0);
    }

    private void setOverhandOffset(Transform overhandTransform, float wristRotation)
    {
        Vector3 offset = bottleHandParent.position - handRoot.position;

        offset = Quaternion.Euler(0, 90 + wristRotation, 90) * offset;
        overhandTransform.Translate(offset);
        overhandTransform.Rotate(-90, 0 + wristRotation, 0);
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
        if (currentMovement == MovementType.SIDE && getPreviousMovement() == MovementType.FRONT
            || currentMovement == MovementType.FRONT && getPreviousMovement() == MovementType.SIDE)
        {
            doParabolaUpdate("y");
        }
        else if (currentMovement == MovementType.FRONT && underhandFront)
        {
            doParabolaUpdate("z");

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
                if (underhandFront)
                {
                    return getPreviousMovement() == MovementType.SIDE ? front.GetChild(1) : front.GetChild(0);
                }
                else
                {
                    return front.GetChild(1);
                }
            case MovementType.SIDE:
                if (underhandFront)
                {
                    return getPreviousMovement() == MovementType.FRONT ? side.GetChild(1) : side.GetChild(0);
                }
                else
                {
                    return side.GetChild(1);
                }
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
            if (currentMovement == MovementType.FRONT && (Vector3.Distance(transform.position, front.GetChild(0).position) < 0.01f || Vector3.Distance(transform.position, front.GetChild(1).position) < 0.01f))
            {
                MovementType previousMovement = getPreviousMovement();
                if (bottleGrabbed)
                {
                    detectFrontBottleDrop();
                    fingerAnimator.executeOpenGrip(0.15f);
                }
                else
                {
                    detectFrontBottleGrab();
                    fingerAnimator.executeCloseGrip(0.15f);

                }
                currentMovementIndex += 1;
                if (currentMovementIndex < movementSequence.Count)
                {
                    currentMovement = (MovementType)movementSequence[currentMovementIndex];
                    if (underhandFront)
                    {
                        initCurrentMovement(previousMovement == MovementType.SIDE ? front.GetChild(1) : front.GetChild(0), getCurrentMovementTarget());
                    }
                    else
                    {
                        initCurrentMovement(front.GetChild(1), getCurrentMovementTarget());
                    }
                }
            }

            if (currentMovement == MovementType.SIDE && (Vector3.Distance(transform.position, side.GetChild(0).position) < 0.01f || Vector3.Distance(transform.position, side.GetChild(1).position) < 0.01f))
            {
                MovementType previousMovement = getPreviousMovement();
                if (bottleGrabbed)
                {
                    detectSideBottleDrop();
                    fingerAnimator.executeOpenGrip(0.15f);
                }
                else
                {
                    detectSideBottleGrab();
                    fingerAnimator.executeCloseGrip(0.15f);

                }
                currentMovementIndex += 1;
                if (currentMovementIndex < movementSequence.Count)
                {
                    currentMovement = (MovementType)movementSequence[currentMovementIndex];
                    if (underhandFront)
                    {
                        initCurrentMovement(previousMovement == MovementType.FRONT ? side.GetChild(1) : side.GetChild(0), getCurrentMovementTarget());
                    }
                    else
                    {
                        initCurrentMovement(side.GetChild(1), getCurrentMovementTarget());
                    }
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
                underhandFront = false;
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
