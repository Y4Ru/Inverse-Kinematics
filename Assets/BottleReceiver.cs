using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleReceiver : MonoBehaviour
{

    public GameObject armTarget;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        if (armTarget.GetComponent<ArmTrajectory>().GetCurrentMovementType() == MovementType.NO_MOVEMENT && other.gameObject.tag == "Target" && !other.gameObject.GetComponent<GraspableObject>().IsGrabbed())
        {
            other.gameObject.transform.position = this.transform.position;
            other.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}