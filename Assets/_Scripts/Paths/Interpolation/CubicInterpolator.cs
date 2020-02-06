using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicInterpolator : AbstractInterpolator
{
    public CubicInterpolator(float[] array) : base(array) { }

    private float m_tangentFactor = 1;
    private float GetTangent(float k) => m_tangentFactor * (GetClippedInput((int)k + 1) - GetClippedInput((int) k - 1)) / 2;
    public override float Interpolate(float t)
    {
        float k, t2, t3;
        float[] m = new float[2];
        float[] p = new float[2];

        k = Mathf.Floor(t);
        m[0] = GetTangent(k);
        m[1] = GetTangent(k + 1);
        p[0] = GetClippedInput((int)k);
        p[1] = GetClippedInput((int)k + 1);
        t -= k;
        t2 = t * t;
        t3 = t2 * t;
        return (2 * t3 - 3 * t2 + 1) * p[0] + (t3 - 2 * t2 + t) * m[0] + (-2 * t3 + 3 * t2) * p[1] + (t3 - t2) * m[1];
    }
}
