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

    public void executeOpenGrip()
    {
        pinkyTrajectory.executeOpenFinger();
        ringTrajectory.executeOpenFinger();
        middleTrajectory.executeOpenFinger();
        indexTrajectory.executeOpenFinger();
        thumbTrajectory.executeOpenFinger();
    }
    public void executeCloseGrip()
    {
        pinkyTrajectory.executeCloseFinger();
        ringTrajectory.executeCloseFinger();
        middleTrajectory.executeCloseFinger();
        indexTrajectory.executeCloseFinger();
        thumbTrajectory.executeCloseFinger();
    }
}
