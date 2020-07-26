using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Transform handTarget;

    public Transform neutral;
    public Transform side;

    public Transform fingerTarget;

    public Transform bottle;

    public Transform bottleGrabAnchor;

    public Transform bottleHandParent;
    private ArmTrajectory armTrajectory = null;

    private GripAnimator fingerAnimator;


    void Start()
    {
        armTrajectory = handTarget.GetComponent<ArmTrajectory>();
        fingerAnimator = fingerTarget.GetComponent<GripAnimator>();
        //armTrajectory.executeMovement(new ArrayList { MovementType.FRONT, MovementType.SIDE, MovementType.NEUTRAL }, false);
    }

    void Update()
    {
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
            armTrajectory.executeMovement(new ArrayList { MovementType.FRONT, MovementType.SIDE, MovementType.NEUTRAL }, true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            armTrajectory.executeMovement(new ArrayList { MovementType.SIDE, MovementType.FRONT, MovementType.NEUTRAL }, true);
        }


        if (Vector3.Distance(bottleGrabAnchor.position, bottleHandParent.position) < 0.01f)
        {
            bottle.parent = bottleHandParent.transform;
            bottle.GetComponent<Rigidbody>().isKinematic = true;

        }

        if (Vector3.Distance(handTarget.position, side.position) < 0.01f)
        {
            bottle.parent = null;
            bottle.GetComponent<Rigidbody>().isKinematic = false;

        }

        if (Vector3.Distance(handTarget.position, neutral.position) < 0.01f)
        {
            bottle.parent = null;
        }
    }
}


