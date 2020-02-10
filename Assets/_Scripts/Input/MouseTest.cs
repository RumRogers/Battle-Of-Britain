using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTest : MonoBehaviour
{
    [SerializeField]
    private Airplane target;
    [SerializeField]
    bool m_mouseDown;
    [SerializeField]
    bool m_mouseDrag;
    [SerializeField]
    List<Vector3> m_waypoints = new List<Vector3>();
    List<GameObject> m_waypointMarkers = new List<GameObject>();
    Vector3 m_lastMousePosition;
    [SerializeField]
    Vector3 m_mousePos;
    [SerializeField]
    float m_distanceThreshold = .06f;
    [SerializeField]
    GameObject m_pathMarkerPrefab;

    // Update is called once per frame
    void Update()
    {
        ManageInput();
        PreviewPath();

    }

    void ManageInput()
    {
        m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_mousePos.y = transform.position.y;

        bool mouseDown = Input.GetMouseButton(0);
        
        if (mouseDown)
        {
            if(!m_mouseDown) // User has just pressed mouse.
            {
                HandleMouseDown();
                ///StopCoroutine(MoveTarget());
                //m_waypoints.Clear();
                //m_lastMousePosition = m_mousePos;   
            }
            else
            {
                if(m_lastMousePosition != m_mousePos) // User is dragging the mouse around.
                {
                    HandleMouseDrag();
                }
            }
        }
        else
        {
            if(m_mouseDown) // User just released mouse button.
            {
                //StartCoroutine(MoveTarget());
                HandleMouseUp();
            }
        }

        m_mouseDown = mouseDown;
    }

    void PreviewPath()
    {
        if(m_waypoints.Count > 1)
        {
            for(int i = 1; i < m_waypoints.Count; ++i)
            {                    
                Debug.DrawLine(m_waypoints[i - 1], m_waypoints[i], Color.red);    
            }
        }
    }

    void BuildPath()
    {
        if (m_waypoints.Count == 1)
        {
            print(m_waypoints[0]);
            InstantiateWaypointMarker(m_waypoints[0]);
        }
        else
        {
            int count = m_waypoints.Count;
            Vector3 penultimateWaypoint = m_waypoints[count - 2];
            Vector3 lastWaypoint = m_waypoints[count - 1];
            
            List<Vector3> intermediateWaypoints = GetIntermediateWaypointsBetween(penultimateWaypoint, lastWaypoint, .5f);
            

            foreach (Vector3 pos in intermediateWaypoints)
            {
                InstantiateWaypointMarker(pos);
            }
        }
    }

    List<Vector3> GetIntermediateWaypointsBetween(Vector3 waypointA, Vector3 waypointB, float sizeOfEachLine)
    {
        List<Vector3> res = new List<Vector3>();
        Vector3 v = waypointB - waypointA;
        int partitionSize = (int)(v.magnitude / sizeOfEachLine);
        Vector3 oneOverPartitionSize = v / partitionSize;

        for (int i = 1; i < partitionSize; i++)
        {
            res.Add(waypointA + (i * oneOverPartitionSize));
        }                
     
        return res;
    }
    void InstantiateWaypointMarker(Vector3 pos)
    {
        GameObject waypointMarker = Instantiate(m_pathMarkerPrefab);
        waypointMarker.transform.position = pos;
        m_waypointMarkers.Add(waypointMarker);
    }
    IEnumerator MoveTarget()
    {
        m_waypoints = Smooth.MakeSmoothCurve(m_waypoints, 1f);
        if (m_waypoints.Count > 1)
        {
            for (int i = 1; i < m_waypoints.Count; ++i)
            {
                //Vector3 dir = (m_waypoints[i] - m_waypoints[i - 1]).normalized;
                target.transform.position = m_waypoints[i];
                yield return new WaitForSeconds(Time.deltaTime * .5f);
            }
        }
    }

    void HandleMouseDown()
    {
        foreach(GameObject waypointMarker in m_waypointMarkers)
        {
            Destroy(waypointMarker);
        }
        m_waypointMarkers.Clear();
        m_waypoints.Clear();        
        m_lastMousePosition = m_mousePos;
    }

    void HandleMouseDrag()
    {
        m_mouseDrag = true;
        if (m_waypoints.Count == 0 || (m_mousePos - m_waypoints[m_waypoints.Count - 1]).magnitude >= m_distanceThreshold)
        {
            m_waypoints.Add(m_lastMousePosition);
            BuildPath();
        }
        m_lastMousePosition = m_mousePos;
    }

    void HandleMouseUp()
    {
        m_waypoints.Add(m_lastMousePosition);
        if (m_mousePos != m_lastMousePosition)
        {
            m_waypoints.Add(m_mousePos);
            BuildPath();
        }

        target.SetDestinations(m_waypoints);
        m_mouseDrag = false;
    }
}