using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Transform handTarget;

    public Transform side;

    public Transform fingerTarget;

    public Transform bottle;

    public Transform bottleHandParent;

    void Start()
    {
        ArmTrajectory armTrajectory = handTarget.GetComponent<ArmTrajectory>();
        armTrajectory.executeMovement(new ArrayList { MovementType.FRONT, MovementType.SIDE, MovementType.NEUTRAL }, false);
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


        if (Vector3.Distance(bottle.position, bottleHandParent.position) < 0.001f)
        {
            bottle.parent = bottleHandParent;
            //fingerAnimator.executeCloseGrip(0.2f);
        }

        if (Vector3.Distance(handTarget.position, side.position) < 0.001f)
        {
            bottle.parent = null;
            //fingerAnimator.executeOpenGrip(0.2f);
        }
    }
}


