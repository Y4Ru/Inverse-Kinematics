using UnityEngine;
using System.Collections;

public class FingerTrajectory : MonoBehaviour
{
    // Transforms to act as start and end markers for the journey.
    public Transform openMarker;
    public Transform closeMarker;

    // Movement duration from start to end marker.
    public float movementDuration = 1.0F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    private bool directionChange;

    private Transform startTrajectory = null;
    private Transform endTrajectory = null;

    void Start()
    {
        // Keep a note of the time the movement started.
        startTime = Time.time;
        transform.rotation = openMarker.rotation;
        transform.position = openMarker.position;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(openMarker.position, closeMarker.position);
    }

    // Move to the target end position.
    void Update()
    {
        doFingerMovement();
    }

    public void executeCloseFinger()
    {
        startTime = Time.time;
        startTrajectory = openMarker;
        endTrajectory = closeMarker;
    }

    public void executeOpenFinger()
    {
        startTime = Time.time;
        startTrajectory = closeMarker;
        endTrajectory = openMarker;
    }

    private void doFingerMovement()
    {
        if (startTrajectory != null && endTrajectory != null)
        {
            if (transform.position == endTrajectory.position)
            {
                startTrajectory = null;
                endTrajectory = null;
            }
            else
            {
                doUpdate();
            }
        }
    }

    private void doUpdate()
    {
        float dynamicSpeed = journeyLength / movementDuration;

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * dynamicSpeed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        Debug.Log(dynamicSpeed);
        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startTrajectory.position, endTrajectory.position, fractionOfJourney);

        transform.rotation = Quaternion.Lerp(startTrajectory.rotation, endTrajectory.rotation, fractionOfJourney);

    }
}