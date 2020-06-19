using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Transform handTarget;

    public Transform side;

    public Transform fingerTarget;

    public GameObject bottle;

    void Start()
    {
        ArmTrajectory armTrajectory = handTarget.GetComponent<ArmTrajectory>();
        armTrajectory.executeMovement(new ArrayList { MovementType.SIDE, MovementType.FRONT, MovementType.NEUTRAL }, true);
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

        if (Vector3.Distance(bottle.transform.position, handTarget.position) < 0.25f)
        {
            bottle.transform.parent = handTarget;
            bottle.transform.position = new Vector3(bottle.transform.position.x, bottle.transform.position.y, bottle.transform.position.z + 0.1f);
            fingerAnimator.executeCloseGrip(1.0f);
        }

        if (Vector3.Distance(handTarget.position, side.position) < 0.05f)
        {
            bottle.transform.parent = null;
            fingerAnimator.executeOpenGrip(0.2f);
        }
    }
}


