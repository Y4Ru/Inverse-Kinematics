using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryInitiator : MonoBehaviour
{
    public Transform startPinky;
    public Transform endPinky;
    public Transform startRing;
    public Transform endRing;
    public Transform startMiddle;
    public Transform endMiddle;
    public Transform startIndex;
    public Transform endIndex;

    public float distance = .5f;

    // Start is called before the first frame update
    void Start()
    {
        setTrajectoryRange(startPinky, endPinky);
        setTrajectoryRange(startRing, endRing);
        setTrajectoryRange(startMiddle, endMiddle);
        setTrajectoryRange(startIndex, endIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setTrajectoryRange(Transform startTransform, Transform endTransform) 
    {
        Vector3 direction = endTransform.position - startTransform.position;
        //double distance = Vector3.Distance(startTransform.position, endTransform.position);
        endTransform.position = startTransform.position + direction.normalized * distance;

    }
}
