using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarParticlesSpawner : MonoBehaviour
{
    enum LineID
    {
        TL_TR,
        BL_BR,
        TL_BL,
        TR_BR
    };

    enum SectorID
    {
        N, NE, E, SE, S, SW, W, NW
    };

    public static RadarParticlesSpawner Instance;
    private Vector3 m_vTL, m_vTR, m_vBL, m_vBR, m_vCenter;    
    private Dictionary<Airplane, GameObject> m_enemyRadarPairs = new Dictionary<Airplane, GameObject>();
    [SerializeField]
    private GameObject m_radarParticlePrefab;
    [SerializeField]
    private float m_DistanceThreshold;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateViewportCoordinates();
        CheckIntersections();
    }

    public void AddEnemy(Airplane airplane)
    {
        if(m_enemyRadarPairs.ContainsKey(airplane))
        {
            return;
        }

        m_enemyRadarPairs.Add(airplane, null);
    }

    public void RemoveEnemy(Airplane airplane)
    {
        m_enemyRadarPairs.Remove(airplane);
    }

    void UpdateViewportCoordinates()
    {
        Vector3 v = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        m_vTL = new Vector3(v.x, 0f, v.z);
        v = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, Screen.height));
        m_vBR = new Vector3(v.x, 0, v.z);
        m_vTR = new Vector3(m_vBR.x, 0, m_vTL.z);
        m_vBL = new Vector3(m_vTL.x, 0, m_vBR.z);

        m_vCenter = new Vector3(m_vTL.x + Mathf.Abs(m_vTL.x - m_vBR.x) / 2, 0f, m_vTL.z - Mathf.Abs(m_vTL.z - m_vBR.z) / 2);
    }

    private Vector3 FindIntersectionWithViewportBounds(Vector3 targetPosition, LineID which)
    {
        Vector3 res = Vector3.zero;

        if (m_vCenter.x == targetPosition.x)
        {
            res.x = m_vCenter.x;
            switch (which)
            {
                case LineID.TL_TR:
                    res.z = m_vTL.z;
                    break;
                case LineID.BL_BR:
                    res.z = m_vBR.z;
                    break;
            }
            return res;
        }


        float m = (m_vCenter.z - targetPosition.z) / (m_vCenter.x - targetPosition.x);
        float b = (m_vCenter.x * targetPosition.z - targetPosition.x * m_vCenter.z) / (m_vCenter.x - targetPosition.x);

        // y = mx + b
        // x = (y - b)/m
        switch (which)
        {
            case LineID.TL_TR:
                res.z = m_vTL.z;
                res.x = (res.z - b) / m;
                break;
            case LineID.BL_BR:
                res.z = m_vBR.z;
                res.x = (res.z - b) / m;
                break;
            case LineID.TL_BL:
                res.x = m_vTL.x;
                res.z = m * res.x + b;
                break;
            case LineID.TR_BR:
                res.x = m_vBR.x;
                res.z = m * res.x + b;
                break;
        }

        return res;
    }

    private bool IsPointInsideViewport(Vector3 p)
    {
        return p.x >= m_vTL.x && p.x < m_vTR.x && p.z <= m_vTL.z && p.z > m_vBL.z;
    }

    private SectorID FindSectorFromPoint(Vector3 p)
    {
        if (p.x > m_vTL.x && p.x < m_vTR.x)
        {
            if (p.z > m_vTL.z)
            {
                return SectorID.N;
            }
            return SectorID.S;
        }

        if (p.z < m_vTL.z && p.z > m_vBL.z)
        {
            if (p.x < m_vTL.x)
            {
                return SectorID.W;
            }
            return SectorID.E;
        }

        if (p.x < m_vTL.x)
        {
            if (p.z > m_vTL.z)
            {
                return SectorID.NW;
            }
            return SectorID.SW;
        }

        if (p.z > m_vTL.z)
        {
            return SectorID.NE;
        }
        return SectorID.SE;
    }

    Vector3 FindClosestIntersectionToViewportCenter(Vector3 targetPosition, LineID line1, LineID line2)
    {
        Vector3 intersection1 = FindIntersectionWithViewportBounds(targetPosition, line1);
        Vector3 intersection2 = FindIntersectionWithViewportBounds(targetPosition, line2);

        return Vector3.Distance(intersection1, m_vCenter) > Vector3.Distance(intersection2, m_vCenter) ? intersection2 : intersection1;
    }

    void CheckIntersections()
    {
        List<Airplane> tmp = new List<Airplane>();

        foreach(KeyValuePair<Airplane, GameObject> pair in m_enemyRadarPairs)
        {
            if(pair.Value == null)
            {
                if(!IsPointInsideViewport(pair.Key.transform.position))
                {
                    tmp.Add(pair.Key);
                    GameObject radarParticle = Instantiate(m_radarParticlePrefab);
                    radarParticle.transform.position = new Vector3(-10000, 0f, -10000);
                    m_enemyRadarPairs[pair.Key] = radarParticle;
                }
            }
            else if (IsPointInsideViewport(pair.Key.transform.position))
            {
                // At the time being, it is not known whether an enemy can leave the area
                // So it's better not to remove it from the map for now
                Destroy(pair.Value);
                m_enemyRadarPairs[pair.Key] = null;
            }
            else
            {
                Airplane airplane = pair.Key;
                Vector3 radarPosition = Vector3.zero;
                float distance = 0;
                
                switch (FindSectorFromPoint(airplane.transform.position))
                {
                    case SectorID.N:
                        radarPosition.x = airplane.transform.position.x;
                        radarPosition.z = m_vTL.z;
                        distance = Mathf.Abs(m_vTL.z - airplane.transform.position.z);                        
                        break;
                    case SectorID.S:
                        radarPosition.x = airplane.transform.position.x;
                        radarPosition.z = m_vBL.z;
                        distance = Mathf.Abs(m_vBR.z - airplane.transform.position.z);
                        break;
                    case SectorID.W:
                        radarPosition.x = m_vTL.x;
                        radarPosition.z = airplane.transform.position.z;
                        distance = Mathf.Abs(m_vTL.x - airplane.transform.position.x);
                        break;
                    case SectorID.E:
                        radarPosition.x = m_vBR.x;
                        radarPosition.z = airplane.transform.position.z;
                        distance = Mathf.Abs(m_vBR.x - airplane.transform.position.x);
                        break;
                    case SectorID.NW:
                        radarPosition = m_vTL;
                        distance = Mathf.Max(Mathf.Abs(m_vTL.z - airplane.transform.position.z), Mathf.Abs(m_vTL.x - airplane.transform.position.x));
                        break;
                    case SectorID.NE:
                        radarPosition = m_vTR;
                        distance = Mathf.Max(Mathf.Abs(m_vTL.z - airplane.transform.position.z), Mathf.Abs(m_vBR.x - airplane.transform.position.x));
                        break;
                    case SectorID.SW:
                        radarPosition = m_vBL;                        
                        distance = Mathf.Max(Mathf.Abs(m_vBR.z - airplane.transform.position.z), Mathf.Abs(m_vTL.x - airplane.transform.position.x));
                        break;
                    case SectorID.SE:
                        radarPosition = m_vBR;
                        distance = Mathf.Max(Mathf.Abs(m_vBR.z - airplane.transform.position.z), Mathf.Abs(m_vBR.x - airplane.transform.position.x));
                        break;
                }

                m_enemyRadarPairs[pair.Key].transform.position = radarPosition;                
                //print()
                m_enemyRadarPairs[pair.Key].transform.localScale = (1 - Mathf.InverseLerp(0, m_DistanceThreshold, distance) + 0.25f) * Vector3.one;
            }
        }
    }
}
