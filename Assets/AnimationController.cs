using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Transform handTarget;

    void Start()
    {
        ArmTrajectory armTrajectory = handTarget.GetComponent<ArmTrajectory>();
        armTrajectory.executeMovement(new ArrayList { MovementType.FRONT, MovementType.SIDE, MovementType.NEUTRAL });
    }
}


