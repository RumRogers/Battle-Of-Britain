using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoother
{
    public Smoother(List<List<float>> arr)
    {
        m_interpolators = new List<CubicInterpolator>();
        m_dimension = arr[0].Count;

        for (int i = 0; i < m_dimension; i++)
        {            
            CubicInterpolator interpolator = new CubicInterpolator(GetColumn(arr, i));
            m_interpolators.Add(interpolator);            
        }
    }

    private List<CubicInterpolator> m_interpolators;
    private int m_dimension;

    private List<float> GetColumn(List<List<float>> array, int col)
    {
        List<float> res = new List<float>();

        for (int i = 0; i < array.Count; i++)
        {
            res.Add(array[i][col]);              
        }

        return res;
    }

    public float[] Smooth(float t)
    {
        float[] res = new float[m_dimension];

        for(int i = 0; i < m_dimension; i++)
        {
            res[i] = m_interpolators[i].Interpolate(t);
        }

        return res;
    }
}