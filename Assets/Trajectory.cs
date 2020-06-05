using UnityEngine;
using System.Collections;

public class Trajectory : MonoBehaviour
{
    // Transforms to act as start and end markers for the journey.
    public Transform startMarker;
    public Transform endMarker;

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
        transform.rotation = startMarker.rotation;
        transform.position = startMarker.position;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
    }

    // Move to the target end position.
    void Update()
    {
        if (transform.position == endMarker.position && !directionChange)
        {
            directionChange = true;
            startTime = Time.time;
            startTrajectory = endMarker;
            endTrajectory = startMarker;
        } else if (transform.position == startMarker.position && !directionChange)
        {
            directionChange = true;
            startTime = Time.time;
            startTrajectory = startMarker;
            endTrajectory = endMarker;
        } else if (transform.position != startMarker.position && transform.position != endMarker.position) {
            directionChange = false;
        }

        float dynamicSpeed = journeyLength / movementDuration;

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * dynamicSpeed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startTrajectory.position, endTrajectory.position, fractionOfJourney);

        transform.rotation = Quaternion.Lerp(startTrajectory.rotation, endTrajectory.rotation, fractionOfJourney);
        
    }
}