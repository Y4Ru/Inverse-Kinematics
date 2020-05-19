using UnityEngine;
using System.Collections;

public class Trajectory2 : MonoBehaviour
{
    // Transforms to act as start and end markers for the journey.
    public Transform startMarker;
    public Transform endMarker;

    // Movement speed in units per second.
    public float speed = 1.0F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

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
        if(transform.position == endMarker.position)
        {
            startTime = Time.time;
            transform.rotation = startMarker.rotation;
            transform.position = startMarker.position;
        } else 
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);

            transform.rotation = Quaternion.Lerp(startMarker.rotation, endMarker.rotation, Time.time * speed);
        }
    }
}