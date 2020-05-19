using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    public GameObject StartPos;
    public GameObject EndPos;
    public GameObject Target;

    public float delta = .1f;

    private Vector3 inc;

    void Start()
    {
        Vector3 dist = EndPos.transform.position - StartPos.transform.position;
        inc = dist * delta;
        Target.transform.position = StartPos.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Vector3.Distance(Target.transform.position, EndPos.transform.position));
        if( Vector3.Distance(Target.transform.position, EndPos.transform.position) > delta)
        {
           Target.transform.position += inc; 
           Target.transform.rotation = Quaternion.Lerp(StartPos.transform.rotation, EndPos.transform.rotation, delta);
        } else {
            Target.transform.position = StartPos.transform.position;
            Target.transform.rotation = StartPos.transform.rotation;

        }
    }
}
