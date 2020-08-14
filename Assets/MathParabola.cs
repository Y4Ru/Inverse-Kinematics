using UnityEngine;
using System;

public class MathParabola
{

    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t, string direction)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        if (direction == "x")
        {
            return new Vector3(f(t) + Mathf.Lerp(start.x, end.x, t), mid.y, mid.z);
        }
        else if (direction == "z")
        {
            return new Vector3(mid.x, mid.y, f(t) + Mathf.Lerp(start.z, end.z, t));
        }
        else
        {
            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }

    }
}