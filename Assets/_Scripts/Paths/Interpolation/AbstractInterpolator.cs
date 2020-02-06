using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractInterpolator
{
    public AbstractInterpolator(List<float> array)
    {
        m_array = array;
        m_length = array.Count;
    }

    protected List<float> m_array;
    protected int m_length;

    protected int ClipClamp(int i, int n) => Mathf.Clamp(i, 0, n - 1);
    protected float ClipHelper(int i) => m_array[ClipClamp(i, m_length)];
    protected float GetClippedInput(int i) => (i >= 0 && i < m_length) ? m_array[i] : ClipHelper(i);

    public abstract float Interpolate(float t);
}
