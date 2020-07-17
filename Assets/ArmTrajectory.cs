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

    private float ParabolaAnimation;

    void Start()
    {
        // Keep a note of the time the movement started.
        startTime = Time.time;
        transform.rotation = neutral.rotation;
        transform.position = neutral.position;
        offset = bottleHandParent.position - handRoot.position;
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
        transform.rotation = neutral.rotation;
        transform.position = neutral.position;
        offset = bottleHandParent.position - handRoot.position;

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
        //startTime = Time.time;

        float dynamicSpeed = journeyLength / movementDuration;

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * dynamicSpeed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        //Debug.Log(fractionOfJourney);

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startTrajectory.position, endTrajectory.position, fractionOfJourney);

        transform.rotation = Quaternion.Lerp(startTrajectory.rotation, endTrajectory.rotation, fractionOfJourney);
    }

    private void doParabolaUpdate()
    {
        ParabolaAnimation += Time.deltaTime * 0.5f;
        //ParabolaAnimation = ParabolaAnimation % 2f;

        // Set our position as a fraction of the distance between the markers.
        transform.position = MathParabola.Parabola(startTrajectory.position, endTrajectory.position, 0.25f, ParabolaAnimation);

        //Debug.Log(Vector3.Distance(transform.position, endTrajectory.position));
        //Debug.Log(endTrajectory.localPosition);

        transform.rotation = Quaternion.Lerp(startTrajectory.rotation, endTrajectory.rotation, ParabolaAnimation);
    }

    private void initCurrentMovement(Transform movementStart, Transform movementTarget)
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
        startTrajectory = movementStart;
        endTrajectory = movementTarget;

        isCurrentMovementInitialized = true;
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
