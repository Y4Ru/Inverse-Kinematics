using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Transform handTarget;

    public Transform side;

    public Transform fingerTarget;

    public Transform bottle;

    public Transform bottleGrabAnchor;

    public Transform bottleHandParent;
    private ArmTrajectory armTrajectory = null;


    void Start()
    {
        armTrajectory = handTarget.GetComponent<ArmTrajectory>();
    }

    void Update()
    {
        GripAnimator fingerAnimator = fingerTarget.GetComponent<GripAnimator>();

        if (Input.GetKeyDown(KeyCode.C))
        {
            fingerAnimator.executeCloseGrip(1.0f);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            fingerAnimator.executeOpenGrip(1.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            armTrajectory.executeMovement(new ArrayList { MovementType.FRONT, MovementType.SIDE, MovementType.NEUTRAL }, false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            armTrajectory.executeMovement(new ArrayList { MovementType.SIDE, MovementType.FRONT, MovementType.NEUTRAL }, false);
            Debug.Log(">A");
        }

        if (Vector3.Distance(bottleGrabAnchor.position, bottleHandParent.position) < 0.001f)
        {
            bottle.parent = bottleHandParent.transform;
        }

        if (Vector3.Distance(handTarget.position, side.position) < 0.001f)
        {
            bottle.parent = null;
            //fingerAnimator.executeOpenGrip(0.2f);
        }
    }
}


