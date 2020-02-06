using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineDescriptor
{
    public SplineDescriptor(List<PathNode> waypoints)
    {
        m_xCoordDist = new List<List<float>>();
        m_zCoordDist = new List<List<float>>();

        for (int i = 0; i < waypoints.Count; i++)
        {
            PathNode pathNode = waypoints[i];
            m_xCoordDist.Add(new List<float>(){ pathNode.Distance, pathNode.Position.x });
            m_zCoordDist.Add(new List<float>() { pathNode.Distance, pathNode.Position.z });
        }

        m_smootherX = new Smoother(m_xCoordDist);
        m_smootherZ = new Smoother(m_zCoordDist);
    }

    private List<List<float>> m_xCoordDist;
    private List<List<float>> m_zCoordDist;
    private Smoother m_smootherX;
    private Smoother m_smootherZ;

    private int m_currentIdx = 0;

    private int GetIdxFromDistance(float dist)
    {
        if(m_currentIdx == m_xCoordDist.Count || dist < m_xCoordDist[m_currentIdx][0])
        {
            m_currentIdx = 0;
        }
        for(; m_currentIdx < m_xCoordDist.Count; ++m_currentIdx)
        {
            float currentDistance = m_xCoordDist[m_currentIdx][0];
            if(currentDistance == dist)
            {
                return m_currentIdx;
            }
            if(currentDistance > dist)
            {
                if(m_currentIdx == 0)
                {
                    return m_currentIdx;
                }

                float numerator = dist - m_xCoordDist[m_currentIdx - 1][0];
                float denominator = currentDistance - m_xCoordDist[m_currentIdx - 1][0];
                return (int)((m_currentIdx - 1) + (numerator / denominator));
            }
        }

        return m_currentIdx;
    }

    public Vector3 GetXZFromDistance(float dist)
    {
        int idx = GetIdxFromDistance(dist);

        return new Vector3(m_smootherX.Smooth(idx)[1], 0f, m_smootherZ.Smooth(idx)[1]);
    }

    public float GetTotalDistance()
    {
        return m_xCoordDist[m_xCoordDist.Count - 1][0];
    }

    public Vector3 GetDestinationPoint()
    {
        return new Vector3(m_xCoordDist[m_xCoordDist.Count - 1][1], 0f, m_zCoordDist[m_zCoordDist.Count - 1][1]);
    }
}
