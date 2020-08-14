using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Transform handTarget;

    public Transform fingerTarget;

    public Transform bottle;

    public Transform front;

    public Transform side;

    private Vector3 frontOriginPos;

    private Vector3 sideOriginPos;

    private ArmTrajectory2 armTrajectory = null;

    private GripAnimator fingerAnimator;



    void Start()
    {
        armTrajectory = handTarget.GetComponent<ArmTrajectory2>();
        fingerAnimator = fingerTarget.GetComponent<GripAnimator>();
        //armTrajectory.executeMovement(new ArrayList { MovementType.FRONT, MovementType.SIDE, MovementType.NEUTRAL }, false);

        frontOriginPos = front.position;
        sideOriginPos = side.position;
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

        if (Input.GetKeyDown(KeyCode.S))
        {
            bottle.position = sideOriginPos;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            bottle.position = frontOriginPos;
        }
    }
}


