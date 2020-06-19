using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripAnimator : MonoBehaviour
{

    public Transform pinky;
    public Transform ring;
    public Transform middle;
    public Transform index;
    public Transform thumb;

    private FingerTrajectory pinkyTrajectory;
    private FingerTrajectory ringTrajectory;
    private FingerTrajectory middleTrajectory;
    private FingerTrajectory indexTrajectory;
    private FingerTrajectory thumbTrajectory;

    void Start()
    {
        pinkyTrajectory = pinky.GetComponent<FingerTrajectory>();
        ringTrajectory = ring.GetComponent<FingerTrajectory>();
        middleTrajectory = middle.GetComponent<FingerTrajectory>();
        indexTrajectory = index.GetComponent<FingerTrajectory>();
        thumbTrajectory = thumb.GetComponent<FingerTrajectory>();
    }

    public void executeOpenGrip(float movementDuration)
    {
        pinkyTrajectory.executeOpenFinger(movementDuration);
        ringTrajectory.executeOpenFinger(movementDuration);
        middleTrajectory.executeOpenFinger(movementDuration);
        indexTrajectory.executeOpenFinger(movementDuration);
        thumbTrajectory.executeOpenFinger(movementDuration);
    }
    public void executeCloseGrip(float movementDuration)
    {
        pinkyTrajectory.executeCloseFinger(movementDuration);
        ringTrajectory.executeCloseFinger(movementDuration);
        middleTrajectory.executeCloseFinger(movementDuration);
        indexTrajectory.executeCloseFinger(movementDuration);
        thumbTrajectory.executeCloseFinger(movementDuration);
    }
}
