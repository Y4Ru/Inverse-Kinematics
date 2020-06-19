using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Transform handTarget;

    public Transform fingerTarget;

    void Start()
    {
        ArmTrajectory armTrajectory = handTarget.GetComponent<ArmTrajectory>();
        armTrajectory.executeMovement(new ArrayList { MovementType.FRONT, MovementType.SIDE, MovementType.NEUTRAL });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GripAnimator fingerAnimator = fingerTarget.GetComponent<GripAnimator>();
            fingerAnimator.executeCloseGrip();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            GripAnimator fingerAnimator = fingerTarget.GetComponent<GripAnimator>();
            fingerAnimator.executeOpenGrip();
        }
    }
}


